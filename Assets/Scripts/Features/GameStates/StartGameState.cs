using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartGameState : IGameState
{
    private Logger _logger;
    
    public StartGameState(Logger logger)
    {
        _logger = logger;
    }

    public async void Enter(object obj)
    {
        _logger.Log("Enter Game State");
        ParticleSystemManager.InstantiateEffectAsyncS("ps", 5).Forget();
    }
    
    public void Update() {}
    public void Exit() {}
}