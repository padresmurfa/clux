using System;
using System.Runtime.CompilerServices;

namespace Clux
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class AbbreviationAttribute : OrderedAttribute
    {
        private readonly char abbreviation;
        public AbbreviationAttribute(char abbreviation, [CallerLineNumber] int lineNumber = 0)
            : base(lineNumber)
        {
            this.abbreviation = abbreviation;
        }

        public char Abbreviation { get { return this.abbreviation; } }
    }
}
