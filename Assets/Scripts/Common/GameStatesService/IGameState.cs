using System.Threading.Tasks;

public interface IGameState
{
    void Enter(object data = null);
    void Update();
    void Exit();
}

/// <summary>
/// Extended interface for asynchronous state transitions.
/// </summary>
public interface IAsyncGameState : IGameState
{
    Task EnterAsync(object data = null);
    Task ExitAsync();
}
