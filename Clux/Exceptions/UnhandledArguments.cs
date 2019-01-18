using System.Collections.Generic;

namespace Clux
{
    public class UnhandledArguments<T> : OptionsException<T> where T : new()
    {
        public UnhandledArguments(IEnumerable<string> unhandled)
            : base(unhandled)
        {
        }
    }
}
