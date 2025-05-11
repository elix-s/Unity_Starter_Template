using Cysharp.Threading.Tasks;
using Common.AudioService;
using Common.SavingSystem;
using Common.UIService;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public async UniTask Enter(StatePayload payload)
    {
        CallTracer.TraceMethodEntry();
        _uiService.ShowLoadingScreen(1500).Forget();
        _logger.Log("Entering MenuState");
        _uiService.ShowUIPanelWithComponent<MainMenuView>("MainMenu").Forget();
    }
    
    public void Update()
    {
        
    }

    public async UniTask Exit()
    {
        CallTracer.TraceMethodEntry();
        _uiService.HideUIPanel();
    }
}
