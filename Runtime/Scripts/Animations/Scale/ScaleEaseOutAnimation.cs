using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Ease Out scale curve.
    /// </summary>
    [Serializable]
    public class ScaleEaseOutConfig : ITweenLogic
    {
        public string LogicName => "Ease Out";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleEaseOutAnimation.LogicID;
    }

    /// <summary>
    /// Executes a scale transformation that starts fast and smoothly decelerates to its target.
    /// </summary>
    public static class ScaleEaseOutAnimation
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
            float smoothProgress = EaseUtility.EvaluateEaseOut(progress);

            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}