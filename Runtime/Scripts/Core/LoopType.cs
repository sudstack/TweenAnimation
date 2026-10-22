namespace SS.TweenAnimations
{
    /// <summary>
    /// Defines the behavior of a looping animation.
    /// </summary>
    public enum LoopType
    {
        // Disables looping completely. The animation will play exactly once.
        None,

        // Restarts the animation from the beginning instantly upon completion.
        Restart,

        // Plays the animation backward and forward seamlessly.
        PingPong
    }
}