using System;
using System.Runtime.InteropServices;
using Common.GameStateService;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    private GameStateService _gameStateService;

    [Inject]
    private void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    private void Awake()
    {
        _startGameButton.onClick.AddListener((() => _gameStateService.ChangeState<StartGameState>()));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
