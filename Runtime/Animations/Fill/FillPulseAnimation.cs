using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// UI config for a breathing/pulsing fill curve using Sine wave math.
    /// Perfect for indeterminate loading spinners and organic UI elements.
    /// </summary>
    [Serializable]
    public class FillPulseConfig : ITweenLogic
    {
        public string LogicName => "Sine Pulse (Loader)";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => FillPulseAnimation.LogicID;
    }

    /// <summary>
    /// Executes an organic fill mutation that expands and contracts seamlessly.
    /// </summary>
    public static class FillPulseAnimation
    {
        public static int LogicID;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            LogicID = TweenEngine.RegisterLogic(CalculateTick);
        }

        public static void CalculateTick(ref TweenState state)
        {
            // Safety check for UI Image
            if (state.UIImage == null) return;

            float progress = Mathf.Clamp01(state.Elapsed / state.Duration);

            // MATH MAGIC: Mathf.Sin(progress * PI) 
            // When progress is 0, Sin = 0. When progress is 0.5, Sin = 1. When progress is 1, Sin = 0.
            // This naturally creates a flawless 'breathe in and out' loop in a single cycle!
            float smoothPulse = Mathf.Sin(progress * Mathf.PI);

            // Interpolate between the starting fill (e.g. 0.1) and target fill (e.g. 0.8)
            state.UIImage.fillAmount = Mathf.Lerp(state.StartVal.x, state.EndVal.x, smoothPulse);
        }
    }
}