using UnityEngine;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A centralized, stateless mathematical utility class.
    /// By keeping all easing equations here, we adhere to the DRY (Don't Repeat Yourself) principle.
    /// Multiple animation logics (Move, Scale, Rotate) can route through these same high-speed functions.
    /// All methods expect a normalized linear progress value between 0.0f and 1.0f.
    /// </summary>
    public static class EaseUtility
    {
        /// <summary>
        /// Linear: Steady pace from start to finish. Zero smoothing.
        /// </summary>
        public static float EvaluateLinear(float progress)
        {
            return progress;
        }

        /// <summary>
        /// Ease In: Starts slow, then accelerates over time.
        /// Formula: t^2
        /// </summary>
        public static float EvaluateEaseIn(float progress)
        {
            return progress * progress;
        }

        /// <summary>
        /// Ease Out: Starts fast, then gracefully decelerates towards the target.
        /// Formula: 1 - (1 - t)^2
        /// </summary>
        public static float EvaluateEaseOut(float progress)
        {
            return 1f - (1f - progress) * (1f - progress);
        }

        /// <summary>
        /// Ease In Out: Starts slow, accelerates in the middle, and decelerates at the end.
        /// </summary>
        public static float EvaluateEaseInOut(float progress)
        {
            return progress < 0.5f
                ? 2f * progress * progress
                : 1f - Mathf.Pow(-2f * progress + 2f, 2f) / 2f;
        }

        /// <summary>
        /// Bounce Out: Simulates a physical bounce/gravity effect at the end of the animation.
        /// Uses standard Robert Penner equations.
        /// </summary>
        public static float EvaluateBounceOut(float progress)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (progress < 1f / d1)
            {
                return n1 * progress * progress;
            }
            else if (progress < 2f / d1)
            {
                return n1 * (progress -= 1.5f / d1) * progress + 0.75f;
            }
            else if (progress < 2.5f / d1)
            {
                return n1 * (progress -= 2.25f / d1) * progress + 0.9375f;
            }
            else
            {
                return n1 * (progress -= 2.625f / d1) * progress + 0.984375f;
            }
        }

        /// <summary>
        /// Elastic Out: Overshoots the target and snaps back into place like a rubber band.
        /// Highly effective for energetic UI popups.
        /// </summary>
        public static float EvaluateElasticOut(float progress)
        {
            const float c4 = (2f * Mathf.PI) / 3f;

            return progress == 0f
                ? 0f
                : progress == 1f
                ? 1f
                : Mathf.Pow(2f, -10f * progress) * Mathf.Sin((progress * 10f - 0.75f) * c4) + 1f;
        }
    }
}