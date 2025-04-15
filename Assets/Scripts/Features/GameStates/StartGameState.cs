using System;
using System.Collections;
using Common.UIService;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartGameState : IGameState
{
    private Logger _logger;
    private UIService _uiService;
    
    public StartGameState(Logger logger, UIService uiService)
    {
        _logger = logger;
        _uiService = uiService;
    }

    public async UniTask Enter(StatePayload payload)
    {
        _uiService.ShowLoadingScreen(1500).Forget();
        
        if (payload is StartGamePayload startGamePayload)
        {
            Debug.Log(startGamePayload.Scores);
        }
       
        _uiService.ShowUIPanelWithComponent<GameUIView>("GameUI").Forget();
    }

    public void Update()
    {
        Debug.Log("Update");
    }

    public async UniTask Exit()
    {
        _uiService.HideUIPanel();
    }
}