using Cysharp.Threading.Tasks;

namespace EventBus
{
    public delegate void DispatchableAction<in T>(T eventInfo) where T : IDispatchableEvent;
    public delegate UniTask DispatchableAsyncAction<in T>(T eventInfo) where T : IDispatchableEvent;
}