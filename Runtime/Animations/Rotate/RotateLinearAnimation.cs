using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The UI configuration for the Linear rotation curve.
    /// </summary>
    [Serializable]
    public class RotateLinearConfig : ITweenLogic
    {
        public string LogicName => "Linear";

        public void Bake(ref TweenState state) { }

        public int GetLogicID() => RotateLinearAnimation.LogicID;
    }

    /// <summary>
    /// Executes a constant-speed rotation transformation without any easing.
    /// </summary>
    public static class RotateLinearAnimation
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

            Vector3 currentRot = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);

            if (state.IsLocal)
                state.Target.localEulerAngles = currentRot;
            else
                state.Target.eulerAngles = currentRot;
        }
    }
}