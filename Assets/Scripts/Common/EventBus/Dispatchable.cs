using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace EventBus
{
    internal class Dispatchable
    {
        internal readonly List<Delegate> handlers = new List<Delegate>();
        internal readonly List<object> listeners = new List<object>();

        internal int Count => listeners.Count;

        internal void Add(object listener, Delegate handler)
        {
            Assert.AreEqual(handlers.Count, listeners.Count);
            handlers.Add(handler);
            listeners.Add(listener);
        }

        internal void Remove(object listener = null, Delegate handler = null)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                var listenerSameOrNull = listener == null || listener == listeners[i];
                var handlerSameOrNull = handler == null || handler == handlers[i];

                if (listeners[i] != null && listenerSameOrNull && handlerSameOrNull)
                    RemoveAt(i);
            }
        }

        internal void RemoveAt(int i)
        {
            Assert.AreEqual(handlers.Count, listeners.Count);
            handlers.RemoveAt(i);
            listeners.RemoveAt(i);
        }
    }
}