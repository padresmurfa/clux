using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Clux
{
    public partial class ParserArg<T>
        where T : new()
    {
        TargetProperty<T> property;
        Type targetType;
        string arg;
        
        public ParserArg(TargetProperty<T> property, Type targetType, string arg)
        {
            this.property = property;
            this.targetType = targetType;
            this.arg = arg;
        }
        
        object ParseEnum()
        {
            var argLower = arg.ToLowerInvariant();

            // TODO: enums may also be stated by value
            var enumValues = System.Enum.GetValues(targetType);
            foreach (var e in enumValues)
            {
                var enumName = System.Enum.GetName(targetType, e);
                if (enumName.ToLowerInvariant() == argLower)
                {
                    return e;
                }
            }

            var underlyingType = System.Enum.GetUnderlyingType(targetType);

            var underlyingValue = new ParserArg<T>(property, underlyingType, arg).ParseArg();

            var valueType = underlyingValue.GetType();

            var retval = Enum.ToObject(targetType, (dynamic)underlyingValue);
            
            foreach (var e in enumValues)
            {
                if (e.Equals(retval))
                {
                    return e;
                }
            }
            
            throw new InvalidOptionValue<T>(property, arg);
        }

        object ParseBool()
        {
            var argLower = arg.ToLowerInvariant();
            
            switch (argLower)
            {
                case "0":
                case "f":
                case "false":
                case "n":
                case "no":
                    return false;
                    
                case "1":
                case "t":
                case "true":
                case "y":
                case "yes":
                    return true;
            }
            
            throw new InvalidOptionValue<T>(property, arg);
        }
        
        object ParseDateTime()
        {
            try
            {
                var dts = new []{
                   "yyyyMMddHHmmssffffff",
                   "yyyyMMddHHmmssfffff",
                   "yyyyMMddHHmmssffff",
                   "yyyyMMddHHmmssfff",
                   "yyyyMMddHHmmssff",
                   "yyyyMMddHHmmssf",
                   "yyyyMMddHHmmss",
                   "yyyyMMddHHmm",
                   "yyyyMMddHH",
                   "yyyyMMdd",
                   "yyyyMM",
                   "yyyy",
                   
                   "d",
                   "D",
                   "f",
                   "F",
                   "g",
                   "G",
                   "m",
                   "M",
                   "r",
                   "s",
                   "t",
                   "T",
                   "u",
                   "U",
                   "y"
                };
                
                List<DateTime> possibilities = new List<DateTime>();
                foreach (var format in dts)
                {
                    if (DateTime.TryParseExact(
                        arg, format, null,
                        System.Globalization.DateTimeStyles.AssumeLocal|System.Globalization.DateTimeStyles.AllowWhiteSpaces|System.Globalization.DateTimeStyles.AdjustToUniversal,
                        out var dateTime))
                    {
                        possibilities.Add(dateTime);
                    }
                }
                
                if (possibilities.Count() == 1)
                {
                    return possibilities.Single();
                }
                else if (possibilities.Any())
                {
                    var first = possibilities.First();
                    if (possibilities.All(x => x.Equals(first)))
                    {
                        return first;
                    }
                    throw new InvalidOptionValue<T>(property, arg);
                }
                else
                {
                    throw new InvalidOptionValue<T>(property, arg);
                }
            }
            catch (FormatException)
            {
                throw new InvalidOptionValue<T>(property, arg);
            }
        }
        
        bool ParseNumeric(out object number)
        {
            if (typeof(sbyte).IsAssignableFrom(targetType))
            {
                number = s2n<sbyte>();
            }
            else if (typeof(short).IsAssignableFrom(targetType))
            {
                number = s2n<short>();
            }
            else if (typeof(int).IsAssignableFrom(targetType))
            {
                number = s2n<int>();
            }
            else if (typeof(long).IsAssignableFrom(targetType))
            {
                number = s2n<long>();
            }
            else if (typeof(byte).IsAssignableFrom(targetType))
            {
                number = s2n<byte>();
            }
            else if (typeof(ushort).IsAssignableFrom(targetType))
            {
                number = s2n<ushort>();
            }
            else if (typeof(uint).IsAssignableFrom(targetType))
            {
                number = s2n<uint>();
            }
            else if (typeof(ulong).IsAssignableFrom(targetType))
            {
                number = s2n<ulong>();
            }
            else if (typeof(double).IsAssignableFrom(targetType))
            {
                number = s2n<double>();
            }
            else if (typeof(float).IsAssignableFrom(targetType))
            {
                number = s2n<float>();
            }
            else if (typeof(decimal).IsAssignableFrom(targetType))
            {
                number = s2n<decimal>();
            }
            else
            {
                number = null;
                return false;
            }
            return true;
        }
        
        public object ParseArg()
        {
            var underlyingNull = Nullable.GetUnderlyingType(targetType);
            targetType = underlyingNull ?? targetType;

            if (targetType.IsEnum)
            {
                return ParseEnum();
            }
            else
            {
                if (typeof(bool).IsAssignableFrom(targetType))
                {
                    return ParseBool();
                }
                else if (typeof(string).IsAssignableFrom(targetType))
                {
                    return arg;
                }
                else if (typeof(char).IsAssignableFrom(targetType))
                {
                    if (arg.Length != 1)
                    {
                        throw new InvalidOptionValue<T>(property, arg);
                    }
                    return arg.First();
                }
                else if (typeof(DateTime).IsAssignableFrom(targetType))
                {
                    return ParseDateTime();
                }
                else if (ParseNumeric(out var number))
                {
                    return number;
                }
                else
                {
                    throw new NotSupportedException("type " + targetType.FullName + " used by " + this.property.Name + " has an unsupported base type");
                }
            }
        }
   
        N s2n<N>() where N : new()
        {
            try
            {
                try
                {
                    return S2N.s2n<N>(arg);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            catch (FormatException)
            {
                throw new InvalidOptionValue<T>(property, arg);
            }
            catch (OverflowException)
            {
                throw new InvalidOptionValue<T>(property, arg);
            }
            catch (Exception)
            {
               throw new InvalidOptionValue<T>(property, arg);
            }
        }
    }
}
