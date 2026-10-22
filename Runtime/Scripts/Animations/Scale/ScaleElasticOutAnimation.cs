using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Elastic Out scale curve.
    /// </summary>
    [Serializable]
    public class ScaleElasticOutConfig : ITweenLogic
    {
        public string LogicName => "Elastic Out";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => ScaleElasticOutAnimation.LogicID;
    }

    /// <summary>
    /// Executes an energetic scale transformation that severely overshoots and snaps back into place.
    /// </summary>
    public static class ScaleElasticOutAnimation
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
            float smoothProgress = EaseUtility.EvaluateElasticOut(progress);

            state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);
        }
    }
}