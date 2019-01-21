using System.Collections.Generic;

namespace Clux
{
    public class UnhandledArguments<T> : OptionsException<T> where T : new()
    {
        public UnhandledArguments(IEnumerable<string> unhandled)
            : base(unhandled)
        {
            var args = string.Join(", ", unhandled);
            
            this.UserErrorMessage = $"Too many arguments, or arguments out of order.  Failed to interpret: '{args}'";
        }
    }
}
