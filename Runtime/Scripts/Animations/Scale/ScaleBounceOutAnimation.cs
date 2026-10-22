using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Bounce Out scale curve.
    /// </summary>
    [Serializable]
    public class ScaleBounceOutConfig : ITweenLogic
    {
        public string LogicName => "Bounce Out";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleBounceOutAnimation.LogicID;
    }

    /// <summary>
    /// Executes a scale transformation that mimics a physical bounce upon reaching the target scale.
    /// Excellent for playful UI popups.
    /// </summary>
    public static class ScaleBounceOutAnimation
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
            float smoothProgress = EaseUtility.EvaluateBounceOut(progress);

            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}