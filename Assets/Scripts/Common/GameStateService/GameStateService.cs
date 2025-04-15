using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.GameStateService
{
    public class GameStateService : IDisposable
    {
        private IGameState _currentState;
        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private readonly Stack<IGameState> _stateStack = new Stack<IGameState>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _stateEntering = false;

        public GameStateService()
        {
            RunLoop(_cts.Token).Forget();
        }

        public void RegisterStates(IEnumerable<IGameState> states)
        {
            foreach (var state in states)
            {
                var type = state.GetType();

                if (_states.ContainsKey(type))
                {
                    Debug.LogWarning($"State {type} is already registered.");
                }
                else
                {
                    _states.Add(type, state);
                }
            }
        }

        public async UniTask ChangeState<T>(StatePayload statePayload = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} is not registered.");
            
            await (_currentState?.Exit() ?? UniTask.CompletedTask);
            _currentState = newState;
            await _currentState.Enter(statePayload);
            
            _stateEntering = true;
        }
        
        public async UniTask PushState<T>(StatePayload statePayload = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} is not registered.");

            if (_currentState != null)
            {
                _stateStack.Push(_currentState);
                await _currentState.Exit();
            }

            _currentState = newState;
            await _currentState.Enter(statePayload);
        }
        
        public async UniTask PopState()
        {
            if (_stateStack.Count > 0)
            {
                await (_currentState?.Exit() ?? UniTask.CompletedTask);
                _currentState = _stateStack.Pop();
                await _currentState.Enter();
            }
            else
            {
                Debug.LogError("State stack is empty. Cannot pop state.");
            }
        }

        private async UniTaskVoid RunLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_stateEntering)
                    {
                        _currentState?.Update();
                    }
                    
                    await UniTask.Yield(cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }
        
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
