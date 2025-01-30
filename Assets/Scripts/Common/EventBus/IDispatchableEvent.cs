namespace EventBus
{
    /// <summary>Base interface for all events.</summary>
    /// <example>
    /// <code>
    /// public interface IMyEvent : IDispatchableEvent
    /// {
    /// }
    /// 
    /// 
    /// [Prewarm(3)]
    /// public class ExampleEvent : IMyEvent
    /// {
    ///     public string foo;
    /// 
    ///     public void Reset()
    ///     {
    ///         foo = null;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IDispatchableEvent
    {
        /// <summary>Called every time the event is returned to the pool.</summary>
        void Reset();
    }
}