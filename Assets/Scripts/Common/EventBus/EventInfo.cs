using System;

namespace EventBus
{
    internal struct EventInfo
    {
        internal Type type;
        internal int prewarmCount;
    }
}