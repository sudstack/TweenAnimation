using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The abstract Master Configuration class using the 'Template Method' design pattern.
    /// Centralizes boilerplate code so child classes remain lightweight.
    /// </summary>
    [Serializable]
    public abstract class TweenConfigBase : ITweenConfig
    {
        public abstract string ConfigName { get; }

        [Header("Target Settings")]
        public GameObject target;

        [Header("Timing Settings")]
        [Min(0f)] public float duration = 1.0f;
        [Min(0f)] public float delay = 0f;

        [Header("Loop Settings")]
        public LoopType loopType = LoopType.None;
        [Tooltip("-1 for infinite looping...")]
        public int loops = 1;

        /// <summary>
        /// The Polymorphic Powerhouse for hot-swapping math curves in the Inspector.
        /// </summary>
        [SerializeReference]
        public ITweenLogic logicConfig;

        public float Duration { get => duration; set => duration = value; }
        public float Delay { get => delay; set => delay = value; }

        public GameObject TargetObject => target;

        /// <summary>
        /// Constructs the unboxed Zero-GC struct directly on the stack.
        /// </summary>
        public TweenState GenerateState()
        {
            if (target == null) return default;

            TweenState state = new TweenState
            {
                Target = target.transform,
                Duration = this.Duration,
                Delay = this.Delay,
                Elapsed = 0f,
                Loops = this.loops,
                LoopsCompleted = 0,
                LoopStyle = this.loopType
            };

            // Let the specific child class inject its unique spatial data.
            ApplyCustomValues(ref state);

            // Bake the curve parameters and map the static calculation ID.
            if (logicConfig != null)
            {
                logicConfig.Bake(ref state);
                state.LogicID = logicConfig.GetLogicID();
            }

            return state;
        }

        /// <summary>
        /// The primary API entry point for triggering animations.
        /// </summary>
        public void Play(ITweenListener listener = null)
        {
            if (target == null) return;

            TweenState state = GenerateState();

            // Cache the component reference to avoid native C++ interop overhead per-frame.
            state.Listener = target.GetComponent<ITweenListener>();

            TweenEngine.Play(state);
        }

        /// <summary>
        /// Contract for child classes to map their specific inspector values into the engine's state struct.
        /// </summary>
        protected abstract void ApplyCustomValues(ref TweenState state);
    }
}