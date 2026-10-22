using UnityEngine;
using UnityEngine.Events; 

namespace SS.TweenAnimations
{
    /// <summary>
    /// The "Event Bridge" pattern implementation.
    /// This MonoBehaviour acts as a bridge between the hardcore Zero-GC engine and the Unity Inspector.
    /// By implementing ITweenListener, it catches the interface-driven callback from the PlayerLoop 
    /// and translates it into a designer-friendly UnityEvent.
    /// 
    /// Performance Note: While UnityEvents do generate a tiny amount of Garbage (GC) via reflection, 
    /// this invoke happens STRICTLY ONCE at the end of the animation, NOT per-frame. 
    /// This makes it a perfect, highly-optimized compromise between raw performance and designer UX.
    /// </summary>
    public class TweenCallbackEvents : MonoBehaviour, ITweenListener
    {
        [Header("What happens when animation finishes?")]
        // Simple event for standard UI/logic flow (e.g., Play Sound, Disable Object).
        public UnityEvent onTweenComplete;

        [Header("Advanced (Passes the animated object)")]
        // Smart event that passes the specific transform that just finished animating.
        // Useful for spawning particles exactly at the target's final resting position.
        public UnityEvent<Transform> onTweenCompleteWithTarget;

        /// <summary>
        /// Invoked by TweenEngine precisely on the final frame of the animation duration.
        /// </summary>
        public void OnTweenComplete(Transform target)
        {
            if (onTweenComplete != null)
            {
                onTweenComplete.Invoke();
            }

            if (onTweenCompleteWithTarget != null)
            {
                onTweenCompleteWithTarget.Invoke(target);
            }
        }
    }
}