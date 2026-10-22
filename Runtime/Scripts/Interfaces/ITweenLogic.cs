namespace SS.TweenAnimations
{
    /// <summary>
    /// The architectural contract for defining animation logic (e.g., Ease In, Ease Out).
    /// Used heavily by the custom Inspector Drawer to populate polymorphic UI dropdowns, 
    /// while completely decoupling the UI data from the runtime math engine.
    /// </summary>
    public interface ITweenLogic
    {
        // The display name that appears in the Unity Inspector dropdown.
        string LogicName { get; }

        /// <summary>
        /// Responsible for injecting specific UI configurations (like custom floats or curves) 
        /// into the master struct BEFORE it goes into the engine loop. 
        /// Passed by 'ref' to avoid struct copying and maintain Zero-GC compliance.
        /// </summary>
        void Bake(ref TweenState state);

        /// <summary>
        /// The crucial bridge between the Inspector and the High-Performance Engine.
        /// Returns the unique integer ID representing the static math calculation method 
        /// mapped inside the TweenEngine router array.
        /// </summary>
        int GetLogicID();
    }
}
