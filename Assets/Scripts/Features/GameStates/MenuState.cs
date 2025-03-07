using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuState : IGameState
{
    private Logger _logger;
    private SceneLoader _sceneLoader;
    private UIService _uiService;
    private AudioService _audioService;
    private SavingSystem _savingSystem;
    
    public MenuState(Logger logger, SceneLoader sceneLoader, UIService uiService, AudioService audioService,
        SavingSystem savingSystem)
    {
        _logger = logger;
        _sceneLoader = sceneLoader;
        _uiService = uiService;
        _audioService = audioService;
        _savingSystem = savingSystem;
    }

    public void Enter(object obj)
    {
        _logger.Log("Entering MenuState");
        _uiService.ShowMainMenu().Forget();
        _audioService.InstantiateAudioSources().Forget();
    }
    
    public void Update()
    {
        
    }
    
    public void Exit() {}
}
