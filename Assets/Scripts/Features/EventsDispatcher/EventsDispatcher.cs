using EventBus;

public class EventsDispatcher 
{
    public interface IStateMachineEvent : IDispatchableEvent { }
    public Dispatcher<IStateMachineEvent> StateDispatcher { get; } = new("StateDispatcher");
}
