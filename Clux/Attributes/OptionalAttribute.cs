using System;
using System.Runtime.CompilerServices;

namespace Clux
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OptionalAttribute : OrderedAttribute
    {
        public OptionalAttribute([CallerLineNumber]int lineNumber = 0)
            : base(lineNumber)
        {
        }
    }
}
