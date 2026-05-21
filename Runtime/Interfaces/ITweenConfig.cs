using UnityEngine;

namespace SS.TweenAnimations
{
    /// <summary>
    /// The master contract for all animation configurations (e.g., Move, Scale, Rotate).
    /// It defines how designer-facing Inspector data translates into raw, engine-ready data.
    /// </summary>
    public interface ITweenConfig
    {
        // The GameObject that will be animated by this configuration.
        GameObject TargetObject { get; }

        // Universal timing properties required by all animations.
        float Duration { get; set; }
        float Delay { get; set; }

        /// <summary>
        /// The factory method that constructs the Zero-GC value-type struct.
        /// It captures the current transform data (Start/End values) and prepares the state 
        /// right before pushing it into the execution array.
        /// </summary>
        TweenState GenerateState();

        /// <summary>
        /// The main entry point to trigger the animation. 
        /// Pushes the generated struct into the TweenEngine's PlayerLoop array.
        /// </summary>
        void Play(ITweenListener listener = null);
    }
}