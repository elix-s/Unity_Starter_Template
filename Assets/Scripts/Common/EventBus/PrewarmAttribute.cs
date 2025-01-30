using System;
using JetBrains.Annotations;

namespace EventBus
{
    /// <summary>Set initial pool capacity for an event.</summary>
    [BaseTypeRequired(typeof(IDispatchableEvent)), AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PrewarmAttribute : Attribute
    {
        /// <summary>The value greater than or equal to 1.</summary>
        internal readonly int count;

        /// <summary>Creates <paramref name="count"/> instances of event in object pool.</summary>
        /// <param name="count">The value must be greater than or equal to 1.</param>
        public PrewarmAttribute(int count)
        {
            this.count = Math.Max(count, 1);
        }
    }
}