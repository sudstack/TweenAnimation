using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Ease In scale curve.
    /// </summary>
    [Serializable]
    public class ScaleEaseInConfig : ITweenLogic
    {
        public string LogicName => "Ease In";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleEaseInAnimation.LogicID;
    }

    /// <summary>
    /// Executes a scale transformation that starts slowly and accelerates over time.
    /// </summary>
    public static class ScaleEaseInAnimation
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
            float smoothProgress = EaseUtility.EvaluateEaseIn(progress);

            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}