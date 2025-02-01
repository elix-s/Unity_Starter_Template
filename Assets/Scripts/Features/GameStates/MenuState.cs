using Cysharp.Threading.Tasks;

public class MenuState : IGameState
{
    private Logger _logger;
    private SceneLoader _sceneLoader;
    private UIService _uiService;
    private AudioService _audioService;
    
    public MenuState(Logger logger, SceneLoader sceneLoader, UIService uiService, AudioService audioService)
    {
        _logger = logger;
        _sceneLoader = sceneLoader;
        _uiService = uiService;
        _audioService = audioService;
    }

    public void Enter()
    {
        _logger.Log("Entering MenuState");
        _uiService.ShowMainMenu().Forget();
        _audioService.InstantiateAudioSource().Forget();
    }
    
    public void Update()
    {
        
    }
    
    public void Exit() {}
}
