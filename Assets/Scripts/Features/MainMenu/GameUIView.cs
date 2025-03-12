using Common.GameStateService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameUIView : MonoBehaviour
{
    [SerializeField] private Button _toMenuButton;
    private GameStateService _gameStateService;

    [Inject]
    private void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    private void Awake()
    {
        _toMenuButton.onClick.AddListener((() => _gameStateService.ChangeState<MenuState>().Forget()));
    }
}
