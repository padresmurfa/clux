using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Clux
{
    // the actual public interface
    public partial class ParserInstance<T> : IParserUnionParserInstance
        where T : new()
    {
        public ParserInstance()
        {
            if (IsParserUnion)
            {
                this.All = new List<TargetProperty<T>>();
            }
            else
            {
                this.All = TargetProperty<T>.TargetPropertiesAndFields.ToList();
            }
            
            this.ByPosition = new ParserInstancePositionalOptions<T>(this.All);
            this.ByShortOption = new ParserInstanceShortOptions<T>(this.All);
            this.ByLongOption = new ParserInstanceLongOptions<T>(this.All);
        }

        public T Parse(params string[] args)
        {
            string[] remainder;
            
            if (IsParserUnion)
            {
                return ParseUnion(out remainder, args);
            }
            
            return Parse(false, out remainder, args);
        }
        
        public T Parse(out string[] remainder, params string[] args)
        {
            if (IsParserUnion)
            {
                return ParseUnion(out remainder, args);
            }
            
            return Parse(true, out remainder, args);
        }

        public string GetHelpMessage(string command)
        {
            return ParserHelp<T>.GetHelpMessage(command, this.All, this.ByPosition, this.ByShortOption, this.ByLongOption);
        }
    }
    
    // parser union specific code:
    public partial class ParserInstance<T>
    {
       void IParserUnionParserInstance.Parse(IParserUnion union, params string[] args)
        {
            var result = Parse(args);
            union.SetResult(result);
        }

        void IParserUnionParserInstance.Parse(IParserUnion union, out string[] remainder, params string[] args)
        {
            var result = Parse(out remainder, args);
            union.SetResult(result);
        }
        
        bool IsParserUnion
        {
            get
            {
                return typeof(IParserUnion).IsAssignableFrom(typeof(T));
            }
        }
        
        T ParseUnion(out string[] remainder, params string[] args)
        {
            var tmp = new T();
            List<ParserException> pexs = new List<ParserException>();
            foreach (var parser in ((IParserUnion)tmp).Parsers)
            {
                try
                {
                    parser.Parse((IParserUnion)tmp, out remainder, args);
                    return tmp;
                }
                catch (ParserException pex)
                {
                    pexs.Add(pex);
                }
            }
            
            throw new AllUnderlyingParsersFailed(pexs);
        }
    }
    
    // parsing code
    public partial class ParserInstance<T>
    {
        List<TargetProperty<T>> All;
        ParserInstancePositionalOptions<T> ByPosition;
        ParserInstanceShortOptions<T> ByShortOption;
        ParserInstanceLongOptions<T> ByLongOption;

        void Reset()
        {
            foreach (var p in this.All)
            {
                p.Touched = false;
                p.Passed = false;
            }
        }
            
        T Parse(bool returnRemainder, out string[] remainder, params string[] args)
        {
            remainder = null;

            Reset();
            var position = 0;

            var target = new T();
            var current = args.ToList();

            try
            {
                while (current.Any() && this.All.Any(x => !x.Passed))
                {
                    var first = current.First();
                    if (this.ByShortOption.AreMergedShortOptions(first))
                    {
                        current = this.ByShortOption.SplitMergedShortOptions(current);
                    }
                    else if (this.ByShortOption.GetShortOption(first, out var shortOption))
                    {
                        current = ApplyOption(current, shortOption, target);
                    }
                    else if (this.ByLongOption.GetLongOption(first, out var longOption))
                    {
                        current = ApplyOption(current, longOption, target);
                    }
                    else
                    {
                        var positionalOption = this.ByPosition.Get(position++, this.All);

                        current = ApplyOption(current, positionalOption, target);
                    }
                }

                if (returnRemainder)
                {
                    remainder = current.ToArray();
                }
                
                AssertConstants();
            }
            catch (ParserException)
            {
                if (!returnRemainder)
                {
                    throw;
                }
                remainder = current.ToArray();
            }

            var missing = this.All.FirstOrDefault(x => x.Required && !x.Touched);
            if (missing != null)
            {
                throw new MissingRequiredOption(missing.LongOption);
            }

            return target;
        }
        
        void AssertConstants()
        {
            var constants = this.All.Where(x => x.Constant != null);
            foreach (var constant in constants)
            {
                if (!constant.Touched)
                {
                    throw new MissingConstantOption(constant.LongOption);
                }
            }
        }

        List<string> ApplyOption(List<string> args, TargetProperty<T> property, T target)
        {
            if (property.TargetType.IsArray)
            {
                return ApplyArrayOption(args, property, target);
            }
            else if (typeof(bool).IsAssignableFrom(property.TargetType) || typeof(bool?).IsAssignableFrom(property.TargetType))
            {
                return ApplyBooleanOption(args, property, target);
            }
            else
            {
                return ApplyPositionalOption(args, property, target);
            }
        }

        private List<string> ApplyPositionalOption(List<string> args, TargetProperty<T> property, T target)
        {
            if (property.Touched)
            {
                throw new DuplicateOption(property.LongOption);
            }

            string sArg;
            args = GetPositionalValue(property.Name, property.Position.HasValue, args, out sArg);

            var arg = new ParserArg<T>(property.LongOption, property.TargetType, sArg).ParseArg();
            property.SetValue(target, arg);
            
            return args;
        }

        private static List<string> ApplyBooleanOption(List<string> args, TargetProperty<T> property, T target)
        {
            if (!property.Position.HasValue)
            {
                args.RemoveAt(0);
            }

            if (property.Touched)
            {
                throw new DuplicateOption(property.LongOption);
            }
            property.SetValue(target, true);
            
            return args;
        }

        private List<string> ApplyArrayOption(List<string> args, TargetProperty<T> property, T target)
        {
            if (!property.Position.HasValue)
            {
                args.RemoveAt(0);
            }

            var elementType = property.TargetType.GetElementType();

            var remainingPositionals = this.ByPosition.Remaining(property.Position);
            var take = args.Count() - remainingPositionals;

            var used = new List<object>();
            foreach (var arg in args.Take(take))
            {
                if (this.ByShortOption.IsShortOption(arg) || this.ByLongOption.IsLongOption(arg))
                {
                    break;
                }

                var parsed = new ParserArg<T>(property.LongOption, elementType, arg).ParseArg();

                used.Add(parsed);
            }

            if (used.Any())
            {
                if (used.Count() < args.Count())
                {
                    args.RemoveRange(0, used.Count());
                }
                else
                {
                    args.Clear();
                }

                var arrayType = elementType.MakeArrayType();

                int previousLength = 0;

                object previous = null;
                if (property.Touched)
                {
                    previous = property.GetValue(target);
                    var arrayLength = arrayType.GetProperty("Length");
                    previousLength = (int)arrayLength.GetValue(previous);
                }

                var array = arrayType
                    .GetConstructor(new[] { typeof(int) })
                     .Invoke(new object[] { used.Count() + previousLength });

                var setItem = arrayType.GetMethod("Set");
                var getItem = arrayType.GetMethod("Get");

                var cursor = 0;
                for (; cursor < previousLength;)
                {
                    var previousElement = getItem.Invoke(previous, new object[] { cursor });
                    setItem.Invoke(array, new[] { cursor++, previousElement });
                }
                foreach (var element in used)
                {
                    setItem.Invoke(array, new[] { cursor++, element });
                }

                property.SetValue(target, array);
            }
            
            return args;
        }

        List<string> GetPositionalValue(string propertyName, bool isPositional, List<string> args, out string val)
        {
            var sKey = args.First();
            args.RemoveAt(0);

            if (isPositional)
            {
                val = sKey;
                return args;
            }
            else
            {
                var vi = sKey.IndexOfAny(new[] { '=' });
                if (vi >= 0)
                {
                    val = sKey.Substring(vi + 1);
                }
                else
                {
                    if (!args.Any())
                    {
                        throw new MissingOptionValue(propertyName);
                    }

                    val = args.First();
                    args.RemoveAt(0);
                }
            }

            return args;
        }
    }
}
