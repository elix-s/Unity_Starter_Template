using Common.GameStateService;
using Common.SavingSystem;
using Cysharp.Threading.Tasks;

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
    
    public async UniTask Enter(StatePayload payload)
    {
        CallTracer.TraceMethodEntry();
        _logger.Log("StartLoadingState");
        _gameState.ChangeState<MenuState>().Forget();
    }

    public void Update(){}
    
    public async UniTask Exit() {}
}
