using System;
using System.Runtime.CompilerServices;

namespace Clux
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class OrderedAttribute : Attribute
    {
        private readonly int lineNumber;

        public OrderedAttribute([CallerLineNumber]int lineNumber = 0)
        {
            this.lineNumber = lineNumber;
        }

        public int Order { get { return lineNumber; } }
    }
}
