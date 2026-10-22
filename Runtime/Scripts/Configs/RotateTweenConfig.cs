using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A concrete data container for Rotation animations.
    /// Maps the target Euler angles from the Inspector to the core engine state.
    /// </summary>
    [Serializable]
    public class RotateTweenConfig : TweenConfigBase
    {
        public override string ConfigName => "Rotate";

        [Space(10)]
        [Header("Animation Specifics")]
        [Tooltip("The final euler angles rotation the object will reach.")]
        public Vector3 targetRotation;

        public bool isLocal = true;

        protected override void ApplyCustomValues(ref TweenState state)
        {
            state.IsLocal = isLocal;

            // Capture the current euler angles dynamically as the starting reference
            state.StartVal = isLocal ? target.transform.localEulerAngles : target.transform.eulerAngles;

            // Map the designer's target rotation angles
            state.EndVal = targetRotation;
        }
    }
}