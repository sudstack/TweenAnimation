using UnityEngine;

namespace SS.TweenAnimations
{
    /// <summary>
    /// A strictly interface-driven callback mechanism. 
    /// Why an Interface? Using standard C# delegates or Actions (e.g., Action<Transform>) 
    /// often generates hidden memory allocations (Garbage) via closures and lambda boxing. 
    /// Storing an Interface reference is 100% Zero-GC, ensuring smooth performance.
    /// </summary>
    public interface ITweenListener
    {
        /// <summary>
        /// Fired by the core engine loop precisely on the frame the animation finishes.
        /// Passes the animated transform back, allowing downstream scripts (like TweenEvents) 
        /// to trigger localized effects, sound, or destruction.
        /// </summary>
        void OnTweenComplete(Transform target);
    }
}