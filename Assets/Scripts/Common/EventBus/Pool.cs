using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EventBus
{
    internal class Pool<V> where V : IDispatchableEvent
    {
        [ShowInInspector] private readonly Dictionary<Type, Stack<V>> _ready = new Dictionary<Type, Stack<V>>();
        [ShowInInspector] private readonly List<V> _busy = new List<V>();
        private readonly string _name;

        internal Pool(string name, HashSet<EventInfo> infoSet)
        {
            _name = name;
            
            foreach (var info in infoSet)
            {
                var stack = CreateStack(info);
                _ready.Add(info.type, stack);
            }
        }
        
        private static Stack<V> CreateStack(EventInfo info)
        {
            var stack = new Stack<V>(info.prewarmCount);
            for (var i = 0; i < info.prewarmCount; i++)
            {
                var element = (V) Activator.CreateInstance(info.type, true);
                stack.Push(element);
            }
            return stack;
        }

        private static T Create<T>() where T : V => (T) Activator.CreateInstance(typeof(T));

        internal T Get<T>() where T : V
        {
            lock (_ready)
            {
                if (!_ready.TryGetValue(typeof(T), out var stack))
                {
                    Debug.LogAssertion($"The '{_name}' pool does not contain an event of type: ${typeof(T)}");
                    return default;
                }

                var element = stack.Count > 0 
                    ? (T) stack.Pop() 
                    : Create<T>();
                    
                _busy.Add(element);
                return element;
            }
        }

        internal void Free<T>(T element) where T : V
        {
            lock (_ready)
            {
                if (!_busy.Contains(element))
                {
                    Debug.LogAssertion($"The '{_name}' Pool does not contain an object of type: ${typeof(T)} in use");
                    return;
                }
                
                element.Reset();
                _busy.Remove(element);
                _ready[typeof(T)].Push(element);
            }
        }
    }
}