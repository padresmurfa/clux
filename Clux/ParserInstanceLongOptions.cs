using System.Collections.Generic;
using System.Linq;

namespace Clux
{
    public class ParserInstanceLongOptions<T>
        where T : new()
    {
        Dictionary<string, TargetProperty<T>> ByLongOption;
        
        public ParserInstanceLongOptions(List<TargetProperty<T>> all)
        {
            this.ByLongOption = all.
                Where(tp => tp.LongOption != null).
                ToDictionary(
                    (tp) => { return tp.LongOption; }
                );
        }        
        
        public bool HasLongOption(string longOption)
        {
            return longOption != null && this.ByLongOption.ContainsKey(longOption);
        }

        public bool IsLongOption(string arg)
        {
            return arg.StartsWith("--") && arg.Length > 2;
        }

        public bool GetLongOption(string arg, out TargetProperty<T> option)
        {
            if (!IsLongOption(arg))
            {
                option = null;
                return false;
            }

            var key = arg.Substring(2).Split(new[] { '=', ':' }).First();
            if (this.ByLongOption.TryGetValue(key, out option))
            {
                if (option.Passed)
                {
                    throw new PassedOption(option.LongOption);
                }
                return true;
            }
            
            throw new UnknownOption(key);
        }
    }
}
