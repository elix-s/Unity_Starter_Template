using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.GameStateService
{
    public class GameStateService
    {
        private IGameState _currentState;
        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private readonly Stack<IGameState> _stateStack = new Stack<IGameState>();
        
        public void RegisterStates(IEnumerable<IGameState> states)
        {
            foreach (var state in states)
            {
                var type = state.GetType();

                if (_states.ContainsKey(type))
                {
                    Debug.LogWarning($"Status {type} is already registered.");
                }
                else
                {
                    _states.Add(type, state);
                }
            }
        }

        public void ChangeState<T>(object data = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} not registered.");

            var previousState = _currentState;
            previousState?.Exit();

            _currentState = newState;
            _currentState.Enter(data);
        }

        /// <summary>
        /// Asynchronous state change. If the state implements IAsyncGameState, asynchronous methods are used.
        /// </summary>
        public async Task ChangeStateAsync<T>(object data = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} not registered.");

            var previousState = _currentState;

            if (previousState is IAsyncGameState asyncPrev)
            {
                await asyncPrev.ExitAsync();
            }
            else
            {
                previousState?.Exit();
            }

            _currentState = newState;

            if (_currentState is IAsyncGameState asyncCurrent)
            {
                await asyncCurrent.EnterAsync(data);
            }
            else
            {
                _currentState.Enter(data);
            }
        }

        public void Update()
        {
            _currentState?.Update();
        }

        /// <summary>
        /// Temporarily switch to a new state while keeping the current one on the stack.
        /// </summary>
        public void PushState<T>(object data = null) where T : IGameState
        {
            if (!_states.TryGetValue(typeof(T), out var newState))
                throw new ArgumentException($"State {typeof(T)} not registered.");

            if (_currentState != null)
            {
                _stateStack.Push(_currentState);
                _currentState.Exit();
            }

            _currentState = newState;
            _currentState.Enter(data);
        }

        /// <summary>
        /// Return to the previous state from the stack.
        /// </summary>
        public void PopState()
        {
            if (_stateStack.Count > 0)
            {
                _currentState?.Exit();
                var previousState = _stateStack.Pop();
                _currentState = previousState;
                _currentState.Enter();
            }
            else
            {
                Debug.LogError("There are no states on the stack to return to..");
            }
        }
    }
}
