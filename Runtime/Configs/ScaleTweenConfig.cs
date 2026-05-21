using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A concrete data container for Scale animations.
    /// Maps the specific target scale from the Inspector to the core engine state.
    /// </summary>
    [Serializable]
    public class ScaleTweenConfig : TweenConfigBase
    {
        public override string ConfigName => "Scale";

        [Space(10)]
        [Header("Animation Specifics")]
        [Tooltip("The final local scale the object will reach.")]
        public Vector3 targetScale = Vector3.one;

        protected override void ApplyCustomValues(ref TweenState state)
        {
            // Capture the current scale dynamically as the starting reference
            state.StartVal = target.transform.localScale;

            // Map the designer's target scale
            state.EndVal = targetScale;
        }
    }
}