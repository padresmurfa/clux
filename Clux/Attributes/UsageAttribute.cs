using System;
using System.Runtime.CompilerServices;

namespace Clux
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class UsageAttribute : OrderedAttribute
    {
        private readonly string usage;
        public UsageAttribute(string usage, [CallerLineNumber] int lineNumber = 0)
            : base(lineNumber)
        {
            this.usage = usage;
        }

        public string Usage { get { return this.usage; } }
    }
}
