using UnityEngine;
using UnityEngine.UI;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A concrete data container for "Fill" animations.
    /// Notice how lightweight this is—because the TweenConfigBase handles all lifecycle, 
    /// memory mapping, and engine routing, this class purely defines 'what' spatial data is changing.
    /// </summary>
    [Serializable]
    public class FillTweenConfig : TweenConfigBase
    {
        // The name for this type of animations.
        public override string ConfigName => "UI Fill";

        [Space(10)]
        [Header("Animation Specifics")]
        [Range(0f, 1f)]
        public float targetFillAmount = 1f;

        /// <summary>
        /// Injects the Fill-specific parameters directly into the master engine state.
        /// </summary>
        protected override void ApplyCustomValues(ref TweenState state)
        {
            state.UIImage = target.GetComponent<Image>();

            if (state.UIImage != null)
            {
                // Float variables ko store karne ke liye hum Vector3.x ka smartly use kar rahe hain
                state.StartVal = new Vector3(state.UIImage.fillAmount, 0, 0);
                state.EndVal = new Vector3(targetFillAmount, 0, 0);
            }
            else
            {
                Debug.LogError($"[TweenEngine] Target {target.name} does not have an Image component!");
            }
        }
    }
}