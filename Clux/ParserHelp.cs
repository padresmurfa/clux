using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Clux
{
    public static class ParserHelp<T>
        where T : new()
    {
        public static string GetHelpMessage(
            string command,
            List<TargetProperty<T>> all,
            ParserInstancePositionalOptions<T> byPosition,
            ParserInstanceShortOptions<T> byShortOption,
            ParserInstanceLongOptions<T> byLongOption)
        {
            if (typeof(IParserUnion).IsAssignableFrom(typeof(T)))
            {
                return GetParseUnionHelpMessage(command);
            }
            
            var usage = $"usage: {command}";
            var prefix=  "   "; 

            var options = new List<string>();
            var usages = new List<string>();
                
            var canMergeShortOption = false;
            foreach (var arg in all.OrderBy(x => x.Order))
            {
                var hasShortOption = byShortOption.HasShortOption(arg.ShortOption);
                var hasLongOption = byLongOption.HasLongOption(arg.LongOption);
                var isConstant = arg.Constant != null;

                var argUsage = DescribeArgUsage(arg.LongOption, arg.TargetType);
                
                if (isConstant)
                {
                    argUsage = $" {arg.Constant}";
                }

                if (hasShortOption && hasLongOption)
                {
                    options.Add($"{prefix}-{arg.ShortOption}, --{arg.LongOption}{argUsage}:");

                    if (canMergeShortOption && string.IsNullOrEmpty(argUsage))
                    {
                        usage = usage.Remove(usage.Length-1) + $"{arg.ShortOption}]";
                    }
                    else
                    {
                        usage += $" [-{arg.ShortOption}{argUsage}]";
                    }

                    canMergeShortOption = string.IsNullOrEmpty(argUsage);
                }
                else if (hasLongOption)
                {
                    options.Add($"{prefix}--{arg.LongOption}{argUsage}:");
                    usage += $" [--{arg.LongOption}{argUsage}]";
                    canMergeShortOption = false;
                }
                else if (isConstant)
                {
                    if (arg.Required)
                    {
                        options.Add($"{prefix}{arg.Constant}:");
                        usage += $" {arg.Constant}";
                    }
                    else
                    {
                        options.Add($"{prefix}[{arg.Constant}]:");
                        usage += $" [{arg.Constant}]";
                    }
                    canMergeShortOption = false;
                }
                else
                {
                    if (argUsage == " <str>")
                    {
                        argUsage = "";
                    }
                    
                    if (arg.Required)
                    {
                        options.Add($"{prefix}<{arg.KebabName}{argUsage}>:");
                        usage += $" <{arg.KebabName}{argUsage}>";
                    }
                    else
                    {
                        options.Add($"{prefix}[{arg.KebabName}{argUsage}]:");
                        usage += $" [{arg.KebabName}{argUsage}]";
                    }
                    canMergeShortOption = false;
                }
                usages.Add(arg.Usage);
            }

            var help = new List<string>();
            var width = options.Max(x => x.Length) + 1;
            for (var i=0;i<options.Count;i++)
            {
                var o = options[i].PadRight(width);
                var u = usages[i];
                help.Add(o + u);
            }
            help.Sort((l,r)=>String.Compare(Sortable(l),Sortable(r)));

            help.Add("");

            return usage + "\n\n" + string.Join("\n", help);
        }
        
        static string GetParseUnionHelpMessage(string command)
        {
            var tmp = new T();
            List<string> submessages = new List<string>();
            
            submessages.Add($"usage: {command} ...");
            submessages.Add("");
            
            foreach (var parser in ((IParserUnion)tmp).Parsers)
            {
                var sm =  parser.GetHelpMessage(command);
                
                var lines = sm.Split('\n');
                lines[0] = lines[0].Replace("usage: ","variant: ");
                
                submessages.Add("");
                submessages.AddRange(lines);
            }
            
            return string.Join("\n", submessages);
        }        

        static string Sortable(string i)
        {
            var s = i.IndexOf(':');
            
            var retval = i.Substring(0, s - 1);
            
            return retval.Replace(" ","").Replace("[","").Replace("-","").Replace("<","");
        }

        static string DescribeArgUsage(string longOption, Type targetType)
        {
            var underlyingNull = Nullable.GetUnderlyingType(targetType);
            targetType = underlyingNull ?? targetType;

            if (targetType.IsEnum)
            {
                var result = " (";

                var enumValues = System.Enum.GetValues(targetType);
                foreach (var e in enumValues)
                {
                    var enumName = System.Enum.GetName(targetType, e);
                    result += enumName.ToLowerInvariant() + "|";
                }
                result = result.Remove(result.Length-1) + ")";

                return result;
            }
            else if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();

                var underlying = DescribeArgUsage(longOption, elementType);

                if (string.IsNullOrEmpty(underlying))
                {
                    return " { <bool> ... }";
                }
                else
                {
                    return $" {{{underlying} ... }}";
                }
            }
            else
            {
                if (typeof(bool).IsAssignableFrom(targetType))
                {
                    return "";
                }
                else if (typeof(string).IsAssignableFrom(targetType))
                {
                    return " <str>";
                }
                else if (typeof(char).IsAssignableFrom(targetType))
                {
                    return " <char>";
                }
                else if (typeof(DateTime).IsAssignableFrom(targetType))
                {
                    return " <datetime>";
                }
                else if (typeof(sbyte).IsAssignableFrom(targetType))
                {
                    return " <int8>";
                }
                else if (typeof(short).IsAssignableFrom(targetType))
                {
                    return " <int16>";
                }
                else if (typeof(int).IsAssignableFrom(targetType))
                {
                    return " <n>";
                }
                else if (typeof(long).IsAssignableFrom(targetType))
                {
                    return " <int64>";
                }
                else if (typeof(byte).IsAssignableFrom(targetType))
                {
                    return " <uint8>";
                }
                else if (typeof(ushort).IsAssignableFrom(targetType))
                {
                    return " <uint16>";
                }
                else if (typeof(uint).IsAssignableFrom(targetType))
                {
                    return " <uint>";
                }
                else if (typeof(ulong).IsAssignableFrom(targetType))
                {
                    return " <uint64>";
                }
                else if (typeof(double).IsAssignableFrom(targetType))
                {
                    return " <double>";
                }
                else if (typeof(float).IsAssignableFrom(targetType))
                {
                    return " <float>";
                }
                else if (typeof(decimal).IsAssignableFrom(targetType))
                {
                    return " <decimal>";
                }
                else
                {
                    throw new NotSupportedException("type " + targetType.FullName + " used by " + longOption + " has an unsupported base type");
                }
            }
        }
    }
}
