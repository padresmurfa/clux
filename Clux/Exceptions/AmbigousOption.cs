using System.Linq;
using System.Collections.Generic;

namespace Clux
{
    public class AmbiguousOption<T> : OptionsException<T> where T : new()
    {
        public AmbiguousOption(IEnumerable<TargetProperty<T>> property)
            : base(property)
        {
        }
    }
}
