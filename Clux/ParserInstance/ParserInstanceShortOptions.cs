using System.Collections.Generic;
using System.Linq;

namespace Clux
{
    public class ParserInstanceShortOptions<T>
        where T : new()
    {
        Dictionary<char, TargetProperty<T>> ByShortOption;
        
        public ParserInstanceShortOptions(List<TargetProperty<T>> all)
        {
            var byShortOption = all.
                Where(tp => tp.ShortOption.HasValue).
                GroupBy(tp => tp.ShortOption);

            this.ByShortOption = new Dictionary<char, TargetProperty<T>>();
            foreach (var shortOption in byShortOption)
            {
                var k = shortOption.Key.Value;
                var v = shortOption.ToList();

                if (v.Count == 1)
                {
                    this.ByShortOption[k] = v.First();
                }
                else if (v.Count(x => x.IsShortOptionExplicit) == 1)
                {
                    this.ByShortOption[k] = v.Single(x => x.IsShortOptionExplicit);
                    foreach (var tp in v.Where(x => !x.IsShortOptionExplicit))
                    {
                        tp.ShortOption = null;
                    }
                }
                else if (v.Any(x => x.IsShortOptionExplicit))
                {
                    throw new AmbiguousOption(k);
                }
                else
                {
                    // else, wasn't asked for, just skip it
                    foreach (var tp in v)
                    {
                        tp.ShortOption = null;
                    }
                }
            }
        }        
        
        public bool HasShortOption(char? shortOption)
        {
            return shortOption != null && this.ByShortOption.ContainsKey(shortOption.Value);
        }

        public bool IsShortOption(string arg)
        {
            return arg.StartsWith("-") && arg.Length > 1 && !arg.StartsWith("--");
        }

        public bool GetShortOption(string arg, out TargetProperty<T> option)
        {
            if (!IsShortOption(arg))
            {
                option = null;
                return false;
            }

            var key = arg.ToCharArray()[1];
            if (this.ByShortOption.TryGetValue(key, out option))
            {
                if (option.Passed)
                {
                    throw new PassedOption(option.ShortOption.Value);
                }
                
                return true;
            }

            throw new UnknownOption(key);
        }
        
        public List<string> SplitMergedShortOptions(List<string> current)
        {
            var merged = current.First();

            var split = merged.Skip(1).Select(arg => $"-{arg}");

            var remainder = current.Skip(1);

            return split.Concat(remainder).ToList();
        }

        public bool AreMergedShortOptions(string arg)
        {
            return this.IsShortOption(arg) && arg.Length > 2;
        }
    }
}
