using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Linear scale curve.
    /// </summary>
    [Serializable]
    public class ScaleLinearConfig : ITweenLogic
    {
        public string LogicName => "Linear";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleLinearAnimation.LogicID;
    }

    /// <summary>
    /// Executes a constant-speed scale transformation without any easing.
    /// </summary>
    public static class ScaleLinearAnimation
    {
        public static int LogicID;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            LogicID = TweenEngine.RegisterLogic(CalculateTick);
        }

        public static void CalculateTick(ref TweenState state)
        {
            float progress = Mathf.Clamp01(state.Elapsed / state.Duration);
            float smoothProgress = EaseUtility.EvaluateLinear(progress);

            // Directly mutate localScale
            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}