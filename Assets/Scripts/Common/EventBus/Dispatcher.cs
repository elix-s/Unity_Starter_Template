using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace EventBus
{
    /// <summary>
    /// Extended implementation of an event bus pattern with event pool.
    /// <list type="bullet">
    ///     <item><description><see cref="Subscribe{T}"/></description></item>
    ///     <item><description><see cref="UnsubscribeListener{T}"/></description></item>
    ///     <item><description><see cref="UnsubscribeListener"/></description></item>
    ///     <item><description><see cref="Get{T}"/></description></item>
    ///     <item><description><see cref="Invoke{T}"/></description></item>
    /// </list>
    /// <para>Use <see cref="Subscribe{T}"/> to add subscription to a specific event for given listener and <see cref="UnsubscribeListener{T}"/> to remove this subscription (or <see cref="UnsubscribeListener"/> to remove all subscriptions for given listener).</para>
    /// <para>To raise an event call <see cref="Get{T}"/> and use received struct for <see cref="Invoke{T}"/>.</para>
    /// <para>To define which events will be available in this dispatcher, create interface what inherits <see cref="IDispatchableEvent"/> and implement it in the desired events.</para>
    /// </summary>
    /// <example><code>
    /// public interface IMyEvent : IDispatchableEvent
    /// {
    /// }
    ///  
    /// [Prewarm(3)]
    /// public class ExampleEvent : IMyEvent
    /// {
    ///     public string foo;
    ///  
    ///     public void Reset()
    ///     {
    ///         foo = null;
    ///     }
    /// }
    ///  
    /// ...
    /// var dispatcher = new Dispatcher&lt;IMyEvent&gt;("Example");
    /// dispatcher.Subscribe&lt;ExampleEvent&gt;(this, ExampleEventHandler);
    /// ...
    /// var event = dispatcher.Pool.Get&lt;ExampleEvent&gt;();
    /// dispatcher.Invoke(event);
    /// ...
    /// dispatcher.Unsubscribe&lt;ExampleEvent&gt;(this);
    /// ...
    /// </code></example>
    public class Dispatcher<V> where V : IDispatchableEvent
    {
        [ShowInInspector] private readonly Pool<V> _pool;
        private readonly Dictionary<Type, Dispatchable> _meta = new Dictionary<Type, Dispatchable>();

        /// <summary>Creates new event dispatcher.</summary>
        /// <typeparam name="V">Interface which defines a family of events to manage.</typeparam>
        /// <param name="name">The name of dispatcher.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not valid (null or empty).</exception>
        public Dispatcher(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be empty");
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var infoSet = new HashSet<EventInfo>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(IsSuitableEvent);
                foreach (var type in types)
                {
                    var prewarmInfo = (PrewarmAttribute) Attribute.GetCustomAttribute(type, typeof(PrewarmAttribute));
                    var eventInfo = new EventInfo { prewarmCount = prewarmInfo?.count ?? 1, type = type};
                    infoSet.Add(eventInfo);
                }
            }
            
            foreach (var info in infoSet)
                RegisterEvent(info);

            _pool = new Pool<V>(name, infoSet);
        }

        #region Internal stuff

        private static bool IsSuitableEvent(Type t) => t.IsClass && !t.IsAbstract && typeof(V).IsAssignableFrom(t);

        private void RegisterEvent(EventInfo info)
        {
            if (_meta.ContainsKey(info.type))
            {
                Debug.LogWarning($"Duplicate key registration for {info.type}");
                return;
            }

            _meta.Add(info.type, new Dispatchable());
        }

        #endregion

        /// <summary>Add a subscription to a specific event for a given listener.</summary>
        /// <param name="listener">The object that will be the listener.</param>
        /// <param name="action">An event callback.</param>
        /// <typeparam name="T">Type of the event.</typeparam>
        [PublicAPI]
        public void Subscribe<T>(object listener, DispatchableAction<T> action) where T : V
        {
            Assert.IsTrue(_meta.ContainsKey(typeof(T)));
            var dispatchableMeta = _meta[typeof(T)];
            dispatchableMeta.Add(listener, action);
        }
        
        /// <summary>Add an async subscription to a specific event for a given listener.</summary>
        /// <param name="listener">The object that will be the listener.</param>
        /// <param name="action">An event async callback.</param>
        /// <typeparam name="T">Type of the event.</typeparam>
        [PublicAPI]
        public void Subscribe<T>(object listener, DispatchableAsyncAction<T> action) where T : V
        {
            Assert.IsTrue(_meta.ContainsKey(typeof(T)));
            var dispatchableMeta = _meta[typeof(T)];
            dispatchableMeta.Add(listener, action);
        }

        /// <summary>Remove a subscription to a specific event for a given listener.</summary>
        /// <param name="listener">Existing listener.</param>
        /// <typeparam name="T">Type of the event.</typeparam>
        [PublicAPI]
        public void UnsubscribeListener<T>(object listener) where T : V
        {
            Assert.IsTrue(_meta.ContainsKey(typeof(T)));
            var dispatchableMeta = _meta[typeof(T)];
            dispatchableMeta.Remove(listener);
        }

        /// <summary>Remove all subscriptions for a given listener.</summary>
        /// <param name="listener">Existing listener.</param>
        [PublicAPI]
        public void UnsubscribeListener(object listener)
        {
            foreach (var dispatchableMeta in _meta.Values)
                dispatchableMeta.Remove(listener);
        }

        /// <summary>Retrieve an event object from the pool.</summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        /// <returns>An event object.</returns>
        [PublicAPI]
        public T Get<T>() where T : V => _pool.Get<T>();

        /// <summary>Raise a given event.</summary>
        /// <param name="metaEvent">An event object.</param>
        /// <typeparam name="T">Type of the event.</typeparam>
        [PublicAPI]
        public async UniTask Invoke<T>(T metaEvent) where T : V
        {
            Assert.IsTrue(_meta.ContainsKey(typeof(T)));
            var dispatchableMeta = _meta[typeof(T)];
            var tasks = new List<UniTask>(dispatchableMeta.Count);
            for (var i = dispatchableMeta.Count - 1; i >= 0; i--)
            {
                var handler = dispatchableMeta.handlers[i];
                var listener = dispatchableMeta.listeners[i];
                var listenerDead = listener is Object unityObject ? unityObject == null : listener == null;

                if (listenerDead)
                {
                    dispatchableMeta.RemoveAt(i);
                    continue;
                }

                try
                {
                    if (handler is DispatchableAsyncAction<T> asyncAction)
                        tasks.Add(asyncAction(metaEvent));
                    else 
                        ((DispatchableAction<T>) handler)(metaEvent);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            await UniTask.WhenAll(tasks.ToArray());
            _pool.Free(metaEvent);
        }
    }
}