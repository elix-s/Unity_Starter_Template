using Cysharp.Threading.Tasks;

public class MenuState : IGameState
{
    private Logger _logger;
    private SceneLoader _sceneLoader;
    private UIService _uiService;
    
    public MenuState(Logger logger, SceneLoader sceneLoader, UIService uiService)
    {
        _logger = logger;
        _sceneLoader = sceneLoader;
        _uiService = uiService;
    }

    public void Enter()
    {
        _logger.Log("Entering MenuState");
        _uiService.ShowMainMenu().Forget();
    }
    
    public void Update()
    {
        
    }
    
    public void Exit() {}
}
