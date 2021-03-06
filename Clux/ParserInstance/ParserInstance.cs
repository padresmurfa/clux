﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Assumptions;

namespace Clux
{
    // the actual public interface
    public partial class ParserInstance<T> : IParserUnionParserInstance
        where T : new()
    {
        private object target;
        
        public ParserInstance()
        {
            try
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
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }

        public T Parse(params string[] args)
        {
            try
            {
                string[] remainder;
                
                if (IsParserUnion)
                {
                    return ParseUnion(out remainder, args);
                }
                
                return Parse(false, out remainder, args);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }
        
        public T Parse(out string[] remainder, params string[] args)
        {
            try
            {
                if (IsParserUnion)
                {
                    return ParseUnion(out remainder, args);
                }
                
                return Parse(true, out remainder, args);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }

        public string GetHelpMessage(string command)
        {
            try
            {
                return ParserHelp<T>.GetHelpMessage(command, this.All, this.ByPosition, this.ByShortOption, this.ByLongOption);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
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
            target = (object)new T();
            List<ParserException> pexs = new List<ParserException>();
            foreach (var parser in ((IParserUnion)target).Parsers)
            {
                try
                {
                    parser.Parse((IParserUnion)target, out remainder, args);
                    return (T)target;
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
            var nextPositional = 0;
            string[] tmpRemainder = args;
            remainder = null;

            Reset();

            target = (object)new T();
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
                        current = ApplyOption(current, shortOption);
                    }
                    else if (this.ByLongOption.GetLongOption(first, out var longOption))
                    {
                        current = ApplyOption(current, longOption);
                    }
                    else
                    {
                        var positionalOption = this.ByPosition.GetPositionalOption(nextPositional++);
                        try
                        {
                            current = ApplyOption(current, positionalOption);
                        }
                        finally
                        {
                            positionalOption.Passed = true;
                            var passed = this.All.Where(o => o.Order < positionalOption.Order);
                            foreach (var p in passed)
                            {
                                p.Passed = true;
                            }
                        }
                    }
                    
                    tmpRemainder = current.ToArray();
                }
                
                AssertConstants();
            }
            catch (ParserException ex)
            {
                if (!returnRemainder)
                {
                    ex.NextPositional = nextPositional;
                    ex.Remainder = tmpRemainder;
                    ex.Input = args;
                    throw;
                }
            }
            
            try
            {
                if (returnRemainder)
                {
                    remainder = tmpRemainder;
                }
                else if (current.Any())
                {
                    throw new UnhandledArguments<T>(current);
                }
    
                var missing = this.All.FirstOrDefault(x => x.Required && !x.Touched);
                if (missing != null)
                {
                    throw new MissingRequiredOption<T>(missing);
                }
                
                return (T)target;
            }
            catch (ParserException ex)
            {
                ex.NextPositional = nextPositional;
                ex.Remainder = tmpRemainder;
                ex.Input = args;
                throw;
            }
        }
        
        void AssertConstants()
        {
            var constants = this.All.Where(x => x.Constant != null && x.Required);
            foreach (var constant in constants)
            {
                if (!constant.Touched)
                {
                    throw new MissingConstantOption<T>(constant);
                }
            }
        }

        List<string> ApplyOption(List<string> args, TargetProperty<T> property)
        {
            var isArray = property.TargetType.IsArray;
            if (isArray)
            {
                var elementType = property.TargetType.GetElementType();
            
                return ApplyCollectionOption(args, property, elementType, CreateOrAppendToArray);
            }
            else if (property.IsBoolean)
            {
                return ApplyBooleanOption(args, property);
            }
            else if (property.TargetType.IsGenericType)
            {
                var elementType = property.TargetType.GenericTypeArguments.First();
                    
                var isList = typeof(IList).IsAssignableFrom(property.TargetType);
                if (isList)
                {
                    return ApplyCollectionOption(args, property, elementType, CreateOrAppendToList);
                }
                
                var hashSetType = typeof(HashSet<>);
                var dude = hashSetType.MakeGenericType(new []{ elementType });
                if (property.TargetType.IsAssignableFrom(dude))
                {
                    return ApplyCollectionOption(args, property, elementType, CreateOrAppendToSet);
                }
            }
            
            return ApplyPositionalOrNamedOption(args, property);
        }

        private List<string> ApplyPositionalOrNamedOption(List<string> args, TargetProperty<T> property)
        {
            if (property.Touched)
            {
                throw new DuplicateOption<T>(property);
            }

            string sArg;
            args = GetPositionalValue(property, property.Position.HasValue, args, out sArg);

            var arg = new ParserArg<T>(property, property.TargetType, sArg).ParseArg();
            property.SetValue(target, arg);
            
            return args;
        }

        private List<string> ApplyBooleanOption(List<string> args, TargetProperty<T> property)
        {
            if (property.Position.HasValue)
            {
                if (property.Touched)
                {
                    throw new DuplicateOption<T>(property);
                }
                
                string sArg;
                args = GetPositionalValue(property, property.Position.HasValue, args, out sArg);
                
                var arg = new ParserArg<T>(property, property.TargetType, sArg).ParseArg();
                property.SetValue(target, arg);
            }
            else
            {
                var value = true;
                var split = args[0].Substring(2).Split(new[] { '=', ':' });
                if (split.Count() == 2)
                {
                    var last = split.Last();
                    value = (bool)new ParserArg<T>(property, property.TargetType, last).ParseArg();
                }
            
                args.RemoveAt(0);
                
                if (property.Touched)
                {
                    throw new DuplicateOption<T>(property);
                }
                
                property.SetValue(target, value);
            }
            
            return args;
        }

        private List<string> ApplyCollectionOption(List<string> args, TargetProperty<T> property, Type elementType, Func<TargetProperty<T>,Type,List<object>,object> createOrAppendToCollection)
        {
            if (!property.Position.HasValue)
            {
                args.RemoveAt(0);
            }

            var remainingPositionals = this.ByPosition.Remaining(property.Position);
            var take = args.Count() - remainingPositionals;

            var used = new List<object>();
            foreach (var arg in args.Take(take))
            {
                if (this.ByShortOption.IsShortOption(arg) || this.ByLongOption.IsLongOption(arg))
                {
                    break;
                }

                var parsed = new ParserArg<T>(property, elementType, arg).ParseArg();

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

                var array = createOrAppendToCollection(property, elementType, used);

                property.SetValue(target, array);
            }

            return args;
        }

        private object CreateOrAppendToArray(TargetProperty<T> property, Type elementType, List<object> append)
        {
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
                 .Invoke(new object[] { append.Count() + previousLength });

            var setItem = arrayType.GetMethod("Set");
            var getItem = arrayType.GetMethod("Get");

            var cursor = 0;
            for (; cursor < previousLength;)
            {
                var previousElement = getItem.Invoke(previous, new object[] { cursor });
                setItem.Invoke(array, new[] { cursor++, previousElement });
            }
            foreach (var element in append)
            {
                setItem.Invoke(array, new[] { cursor++, element });
            }

            return array;
        }
        
        private object CreateOrAppendToList(TargetProperty<T> property, Type elementType, List<object> append)
        {
            IList list = null;
            if (property.Touched)
            {
                list = (IList)property.GetValue(target);
            }
            else
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(elementType);
                list = (IList)Activator.CreateInstance(constructedListType);
            }
            foreach (var item in append)
            {
                list.Add(item);
            }
            return list;
        }

        private object CreateOrAppendToSet(TargetProperty<T> property, Type elementType, List<object> append)
        {
            object collection = null;
            if (property.Touched)
            {
                collection = property.GetValue(target);
            }
            else
            {
                var hashSetType = typeof(HashSet<>);
                var constructedListType = hashSetType.MakeGenericType(elementType);
                collection = Activator.CreateInstance(constructedListType);
            }
            var add = collection.GetType().GetMethod("Add", new Type[]{ elementType });
            foreach (var item in append)
            {
                add.Invoke(collection, new object[]{ item });
            }
            return collection;
        }
        
        List<string> GetPositionalValue(TargetProperty<T> property, bool isPositional, List<string> args, out string val)
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
                var eqIndex = sKey.IndexOfAny(new[] { '=' });
                if (eqIndex >= 0)
                {
                    var valueIndex = eqIndex + 1;
                    if (valueIndex == sKey.Length)
                    {
                        val = string.Empty;
                    }
                    else
                    {
                        val = sKey.Substring(valueIndex);
                    }
                }
                else
                {
                    if (!args.Any())
                    {
                        throw new MissingOptionValue<T>(property);
                    }

                    val = args.First();
                    args.RemoveAt(0);
                }
            }

            return args;
        }
    }
}
