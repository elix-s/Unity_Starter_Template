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

    public void Enter(object obj)
    {
        _uiService.ShowLoadingScreen(1500).Forget();
        _logger.Log("Enter Game State");
        _logger.Log(obj.ToString());
        _uiService.ShowUIPanelWithComponent<GameUIView>("GameUI").Forget();
    }

    public void Update()
    {
        Debug.Log("Update");
    }

    public void Exit()
    {
        _uiService.HideUIPanel().Forget();
    }
}