public class StartLoadingState : IGameState
{
    private GameStateService _gameState;
    private readonly Logger _logger;
    private SavingSystem _savingSystem;
    
    public StartLoadingState(GameStateService gameStateService, Logger logger, SavingSystem savingSystem)
    {
        _gameState = gameStateService;
        _logger = logger;
        _savingSystem = savingSystem;
    }
    
    public void Enter(object obj)
    {
        _logger.Log("StartLoadingState");
        _gameState.ChangeState<MenuState>();
    }

    public void Update(){}
    
    public void Exit() {}
}
