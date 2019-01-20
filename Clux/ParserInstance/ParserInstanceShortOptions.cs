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
                else if (v.Count(x => x.IsShortOptionExplicit) > 1)
                {
                    throw new AmbiguousOption<T>(all.Where(x => x.ShortOption == k));
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
                    throw new PassedOption<T>(option);
                }
                
                return true;
            }

            throw new UnknownOption<T>(new TargetProperty<T>(key));
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
            var couldBe = this.IsShortOption(arg) && arg.Length > 2;
            if (couldBe)
            {
                var splitMerged = arg.Skip(1);
                foreach (var option in splitMerged)
                {
                    if (!this.ByShortOption.ContainsKey(option))
                    {
                        throw new UnknownMergedShortOption<T>(arg.Substring(1), option);
                    }
                    else if (!typeof(bool?).IsAssignableFrom(this.ByShortOption[option].TargetType) && !typeof(bool).IsAssignableFrom(this.ByShortOption[option].TargetType))
                    {
                        throw new NonBoolMergedShortOption<T>(arg.Substring(1), option);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
