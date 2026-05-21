using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SS.TweenAnimations
{
    /// <summary>
    /// Delegate signature for animation math calculations. 
    /// </summary>
    public delegate void TweenTickMethod(ref TweenState state);

    public static class TweenEngine
    {
        // A pre-allocated array acting as a high-speed routing table.
        private static TweenTickMethod[] _logicRouters = new TweenTickMethod[256];
        private static int _nextLogicID = 0;

        /// <summary>
        /// Allows external logic classes to register their static math functions on boot.
        /// </summary>
        public static int RegisterLogic(TweenTickMethod method)
        {
            if (_nextLogicID >= _logicRouters.Length)
            {
                Debug.LogError("[TweenEngine] Router limit reached! Increase array size.");
                return -1;
            }

            int id = _nextLogicID;
            _logicRouters[id] = method;
            _nextLogicID++;
            return id;
        }

        // A contiguous block of memory to store active animations.
        private static TweenState[] _activeTweens = new TweenState[1000];
        private static int _activeCount = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _activeCount = 0;
            _nextLogicID = 0;
            InjectIntoPlayerLoop();
        }

        /// <summary>
        /// Pushes a new animation state into the execution array. O(1) complexity.
        /// </summary>
        public static void Play(TweenState state)
        {
            if (state.Target == null) return;
            if (_activeCount >= _activeTweens.Length)
            {
                Debug.LogWarning("[TweenEngine] Max active tweens reached!");
                return;
            }

            // Insert at the end of the active block to maintain O(1) insertion speed.
            _activeTweens[_activeCount] = state;
            _activeCount++;
        }

        /// <summary>
        /// Safely interrupts and removes all active tweens associated with a specific transform.
        /// </summary>
        public static void Stop(Transform target)
        {
            // Iterating backwards is mandatory when removing elements to prevent index shifting bugs.
            for (int i = _activeCount - 1; i >= 0; i--)
            {
                if (_activeTweens[i].Target == target)
                {
                    RemoveTweenAtIndex(i);
                }
            }
        }

        /// <summary>
        /// Bypasses the standard MonoBehaviour Update cycle by injecting our engine directly 
        /// into Unity's native C++ PlayerLoop.
        /// </summary>
        private static void InjectIntoPlayerLoop()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem customTickSystem = new PlayerLoopSystem
            {
                type = typeof(TweenEngine),
                updateDelegate = UpdateTick
            };

            for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
            {
                if (currentPlayerLoop.subSystemList[i].type == typeof(UnityEngine.PlayerLoop.Update))
                {
                    var updateSubsystems = currentPlayerLoop.subSystemList[i].subSystemList.ToList();
                    updateSubsystems.RemoveAll(s => s.type == typeof(TweenEngine));
                    updateSubsystems.Add(customTickSystem);
                    currentPlayerLoop.subSystemList[i].subSystemList = updateSubsystems.ToArray();
                    break;
                }
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        /// <summary>
        /// The master execution loop. Called natively by Unity every frame.
        /// Highly optimized: No allocations, no boxing, strict array access.
        /// </summary>
        private static void UpdateTick()
        {
            float deltaTime = Time.deltaTime;

            for (int i = 0; i < _activeCount; i++)
            {
                // SAFETY: Clean up gracefully if the target object is destroyed.
                if (_activeTweens[i].Target == null)
                {
                    RemoveTweenAtIndex(i);
                    i--;
                    continue;
                }

                // Delay Check: Pause execution if a start delay is active
                if (_activeTweens[i].Delay > 0)
                {
                    _activeTweens[i].Delay -= deltaTime;
                    continue;
                }

                _activeTweens[i].Elapsed += deltaTime;

                // EXECUTION: Retrieve the LogicID and invoke the static math calculation directly via ref.
                int logicID = _activeTweens[i].LogicID;
                if (_logicRouters[logicID] != null)
                {
                    _logicRouters[logicID](ref _activeTweens[i]);
                }

                // COMPLETION & LOOP EVALUATION
                if (_activeTweens[i].Elapsed >= _activeTweens[i].Duration)
                {
                    // Check if looping is enabled by the designer
                    bool isLoopingEnabled = _activeTweens[i].LoopStyle != LoopType.None;

                    // -1 indicates an infinite loop condition
                    bool isInfinite = _activeTweens[i].Loops == -1;
                    bool hasLoopsLeft = _activeTweens[i].LoopsCompleted < _activeTweens[i].Loops - 1;

                    // Execute loop logic only if explicitly enabled and counts permit
                    if (isLoopingEnabled && (isInfinite || hasLoopsLeft))
                    {
                        _activeTweens[i].LoopsCompleted++;

                        // Maintain perfect frame-timing without losing fractional precision
                        _activeTweens[i].Elapsed -= _activeTweens[i].Duration;

                        // Swap Start and End targets for PingPong logic
                        if (_activeTweens[i].LoopStyle == LoopType.PingPong)
                        {
                            Vector3 temp = _activeTweens[i].StartVal;
                            _activeTweens[i].StartVal = _activeTweens[i].EndVal;
                            _activeTweens[i].EndVal = temp;
                        }
                    }
                    else
                    {
                        // Trigger the zero-allocation interface callback upon final completion
                        if (_activeTweens[i].Listener != null)
                        {
                            _activeTweens[i].Listener.OnTweenComplete(_activeTweens[i].Target);
                        }

                        // Decommission the tween from the active array block
                        RemoveTweenAtIndex(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Performs an O(1) "Swap and Pop" deletion. 
        /// Overwrites the dead element with the last active element, then shrinks the active boundary.
        /// </summary>
        private static void RemoveTweenAtIndex(int index)
        {
            _activeCount--;

            if (index < _activeCount)
            {
                _activeTweens[index] = _activeTweens[_activeCount];
            }

            // Nullify the final slot to prevent memory leaks from lingering references.
            _activeTweens[_activeCount] = default;
        }
    }
}