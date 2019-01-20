using System.Linq;
using System.Collections.Generic;

namespace Clux
{
    public class AmbiguousOption<T> : OptionsException<T> where T : new()
    {
        public AmbiguousOption(IEnumerable<TargetProperty<T>> property, bool isDeclaration = false)
            : base(property)
        {
            var shortOption = this.Options.First().ShortOptionOrDefault;
            var options = this.Options.OrderBy(x => x.Name).Select(x => $"'{x.Name}'").ToList();
            var leading = options.Take(options.Count - 1);
            var optionNames = string.Join(", ", leading) + " or " + options.Last();
            var declaration = (isDeclaration) ? " declaration" : "";
            
            this.UserErrorMessage = $"Ambiguous argument{declaration}. '-{shortOption}' could refer to any of the following: {optionNames}";
        }
    }
}
