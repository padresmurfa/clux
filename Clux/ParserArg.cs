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
        string longOption;
        Type targetType;
        string arg;
        
        public ParserArg(string longOption, Type targetType, string arg)
        {
            this.longOption = longOption;
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

            var underlyingValue = new ParserArg<T>(longOption, underlyingType, arg).ParseArg();

            var valueType = underlyingValue.GetType();

            return Enum.ToObject(targetType, (dynamic)underlyingValue);
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
            
            throw new InvalidOptionValue(longOption);
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
                };

                return DateTime.ParseExact(arg, dts, null, System.Globalization.DateTimeStyles.AssumeLocal|System.Globalization.DateTimeStyles.AllowWhiteSpaces|System.Globalization.DateTimeStyles.AdjustToUniversal);
            }
            catch (FormatException)
            {
                throw new InvalidOptionValue(longOption);
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
                        throw new InvalidOptionValue(longOption);
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
                    throw new NotSupportedException("type " + targetType.FullName + " used by " + longOption + " has an unsupported base type");
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
                throw new InvalidOptionValue(longOption);
            }
            catch (OverflowException)
            {
                throw new InvalidOptionValue(longOption);
            }
            catch (Exception)
            {
               throw new InvalidOptionValue(longOption);
            }
        }
    }
}
