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
        private bool _stateInitialized = false;

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
            
            _currentState?.Exit();

            _currentState = newState;
            _currentState.Enter(statePayload);
            
            await UniTask.Yield();
            _stateInitialized = true;
        }
        
        public void PushState<T>(StatePayload statePayload = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} is not registered.");

            if (_currentState != null)
            {
                _stateStack.Push(_currentState);
                _currentState.Exit();
            }

            _currentState = newState;
            _currentState.Enter(statePayload);
        }
        
        public void PopState()
        {
            if (_stateStack.Count > 0)
            {
                _currentState?.Exit();
                _currentState = _stateStack.Pop();
                _currentState.Enter();
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
                    if (_stateInitialized)
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
