using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The data representation of the "Ease Out" logic curve.
    /// This class is serialized into the Unity Inspector via the ITweenLogic interface.
    /// It acts strictly as a data-holder and router, keeping UI concerns completely separated from the high-speed math engine.
    /// </summary>
    [Serializable]
    public class MoveEaseInOutConfig : ITweenLogic
    {
        // The display name shown in the "Logic Curve" dropdown in the Inspector.
        public string LogicName => "Ease In Out";

        /// <summary>
        /// Bakes any specific UI variables (like custom bounciness or elasticity) into the struct before execution.
        /// Left intentionally blank here because standard EaseOut requires no extra parameters.
        /// </summary>
        public void Bake(ref TweenState state) { }

        /// <summary>
        /// Routes this configuration to the specific static math function mapped inside the core engine.
        /// </summary>
        public int GetLogicID() => MoveEaseInOutAnimation.LogicID;
    }

    /// <summary>
    /// Executes a translation that accelerates at the beginning and decelerates at the end.
    /// The most natural-feeling curve, highly recommended for camera movements or smooth character sliding.
    /// </summary>
    public static class MoveEaseInOutAnimation
    {
        // The unique identifier assigned by the TweenEngine upon boot.
        public static int LogicID;

        /// <summary>
        /// Automatically injects and registers this calculation method into the core engine 
        /// routing array before the first scene loads. This entirely automates engine extensibility.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            LogicID = TweenEngine.RegisterLogic(CalculateTick);
        }

        /// <summary>
        /// The primary mathematical calculation loop executed by the engine every frame.
        /// Passed by 'ref' to ensure the struct memory is mutated directly without allocations.
        /// </summary>
        public static void CalculateTick(ref TweenState state)
        {
            // 1. Calculate the raw, normalized time progress (0f to 1f)
            float progress = Mathf.Clamp01(state.Elapsed / state.Duration);

            // 2. Delegate the heavy mathematical smoothing to the centralized EaseUtility
            float smoothProgress = EaseUtility.EvaluateEaseInOut(progress);

            // 3. Interpolate the spatial data based on the smoothed curve
            Vector3 currentPos = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);

            // 4. Directly manipulate the transform matrix based on the configured coordinate space
            if (state.IsLocal)
                state.Target.localPosition = currentPos;
            else
                state.Target.position = currentPos;
        }
    }
}