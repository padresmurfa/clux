using System;
using System.Runtime.CompilerServices;

namespace Clux
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ConstantAttribute : OrderedAttribute
    {
        private readonly object constant;
        public ConstantAttribute(object constant, [CallerLineNumber] int lineNumber = 0)
            : base(lineNumber)
        {
            this.constant = constant;
        }

        public object Constant { get { return this.constant; } }
    }
}
