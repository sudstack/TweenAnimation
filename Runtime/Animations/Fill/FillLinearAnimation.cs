using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The data representation of the "Linear" logic curve.
    /// This class is serialized into the Unity Inspector via the ITweenLogic interface.
    /// It acts strictly as a data-holder and router, keeping UI concerns completely separated from the high-speed math engine.
    /// </summary>
    [Serializable]
    public class FillLinearConfig : ITweenLogic
    {
        // The display name shown in the "Logic Curve" dropdown in the Inspector.
        public string LogicName => "Linear";

        /// <summary>
        /// Bakes any specific UI variables (like custom bounciness or elasticity) into the struct before execution.
        /// Left intentionally blank here because standard "Linear" requires no extra parameters.
        /// </summary>
        public void Bake(ref TweenState state) { }

        /// <summary>
        /// Routes this configuration to the specific static math function mapped inside the core engine.
        /// </summary>
        public int GetLogicID() => FillLinearAnimation.LogicID;
    }

    /// <summary>
    /// Executes a Linear Fill transformation loop.
    /// </summary>
    public static class FillLinearAnimation
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
            // Safety check in case the image was destroyed or not found
            if (state.UIImage == null) return;

            float progress = Mathf.Clamp01(state.Elapsed / state.Duration);
            float smoothProgress = EaseUtility.EvaluateLinear(progress);

            // Directly mutate the UI fill amount using our packed X variables
            state.UIImage.fillAmount = Mathf.Lerp(state.StartVal.x, state.EndVal.x, smoothProgress);
        }
    }
}