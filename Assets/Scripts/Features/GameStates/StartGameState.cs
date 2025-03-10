public class StartGameState : IGameState
{
    private Logger _logger;

    public StartGameState(Logger logger)
    {
        _logger = logger;
    }

    public void Enter(object obj)
    {
        _logger.Log("Enter Game State");
    }
    
    public void Update() {}
    public void Exit() {}
}