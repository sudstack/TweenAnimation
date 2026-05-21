using UnityEngine;
using System;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A concrete data container for Translation (Movement) animations.
    /// Notice how lightweight this is—because the TweenConfigBase handles all lifecycle, 
    /// memory mapping, and engine routing, this class purely defines 'what' spatial data is changing.
    /// </summary>
    [Serializable]
    public class MoveTweenConfig : TweenConfigBase
    {
        public override string ConfigName => "Move";

        [Space(10)]
        [Header("Animation Specifics")]
        public Vector3 targetPosition;

        // Defines whether the engine should calculate vectors in World Space or Local Space.
        public bool isLocal = true;

        /// <summary>
        /// Injects the movement-specific parameters directly into the master engine state.
        /// </summary>
        protected override void ApplyCustomValues(ref TweenState state)
        {
            state.IsLocal = isLocal;
            state.StartVal = isLocal ? target.transform.localPosition : target.transform.position;
            state.EndVal = targetPosition;
        }
    }
}