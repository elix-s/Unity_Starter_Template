using System;
using System.Collections.Generic;

public class GameStateService 
{
    private IGameState _currentState;
    private Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
    
    public void RegisterStates(IEnumerable<IGameState> states)
    {
        foreach (var state in states)
        {
            _states[state.GetType()] = state;
        }
    }

    public void ChangeState<T>() where T : IGameState
    {
        if (_states.TryGetValue(typeof(T), out var newState))
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }
        else
        {
            throw new ArgumentException($"State {typeof(T)} is not registered.");
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }
}