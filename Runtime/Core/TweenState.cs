using UnityEngine;
using UnityEngine.UI;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The core data container for all animations. 
    /// Defined as a STRUCT (Value Type) instead of a Class (Reference Type) to guarantee Zero-GC allocations.
    /// </summary>
    public struct TweenState
    {
        // The object being animated.
        public Transform Target;

        // Maps to the static logic router array in TweenEngine.
        public int LogicID;

        // Standard structural data required for most lerp/math operations.
        public Vector3 StartVal;
        public Vector3 EndVal;

        // Lifecycle and timing variables.
        public float Duration;
        public float Elapsed;
        public float Delay;
        public bool IsLocal;

        // Looping configuration mapped from the designer's inspector choices.
        public int Loops;
        public int LoopsCompleted;
        public LoopType LoopStyle;

        // Generic padding parameters for custom logic extensions.
        public float customFloat1;
        public float customFloat2;
        public float customFloat3;
        public float customFloat4;

        // UI variables
        public Image UIImage;

        // Interface reference to handle completion callbacks without generating lambda/delegate garbage.
        public ITweenListener Listener;
    }
}