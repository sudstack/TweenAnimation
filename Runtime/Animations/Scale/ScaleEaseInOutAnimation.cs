using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Ease In Out scale curve.
    /// </summary>
    [Serializable]
    public class ScaleEaseInOutConfig : ITweenLogic
    {
        public string LogicName => "Ease In Out";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleEaseInOutAnimation.LogicID;
    }

    /// <summary>
    /// Executes a highly organic scale transformation with smooth acceleration and deceleration.
    /// </summary>
    public static class ScaleEaseInOutAnimation
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
            float smoothProgress = EaseUtility.EvaluateEaseInOut(progress);

            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}