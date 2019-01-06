using System;

namespace Clux
{
    public static class S2N
    {
        public static N s2n<N>(string sVal) where N : new()
        {
            if (string.IsNullOrWhiteSpace(sVal))
            {
                throw new ArgumentException(nameof(sVal));
            }

            if (sVal.Length > 2 && sVal[0] == '0')
            {
                var baseSpecifier = char.ToLowerInvariant(sVal[1]);
                switch (baseSpecifier)
                {
                    case 'b':
                        var b2n = typeof(S2N).GetMethod("b2n", new []{ typeof(string), typeof(N) });
                        if (b2n == null)
                        {
                            throw new NotSupportedException($"s2n does not support {typeof(N).FullName}");
                        }
                        return (N)b2n.Invoke(null, new object[]{ sVal, default(N) });

                    case 'o':
                        var o2n = typeof(S2N).GetMethod("o2n", new []{ typeof(string), typeof(N) });
                        if (o2n == null)
                        {
                            throw new NotSupportedException($"s2n does not support {typeof(N).FullName}");
                        }
                        return (N)o2n.Invoke(null, new object[]{ sVal, default(N) });

                    case 'x':
                        var x2n = typeof(S2N).GetMethod("x2n", new []{ typeof(string), typeof(N) });
                        if (x2n == null)
                        {
                            throw new NotSupportedException($"s2n does not support {typeof(N).FullName}");
                        }
                        return (N)x2n.Invoke(null, new object[]{ sVal, default(N) });
                }
            }

            var method = typeof(N).GetMethod("Parse", new Type[]{ typeof(string) });
            if (method == null)
            {
                throw new NotSupportedException($"s2n does not support {typeof(N).FullName}");
            }

            return (N)method.Invoke(null, new object[]{ sVal });
        }

        static string SkipPrefix(string prefix, string val)
        {
            if (val.Length > prefix.Length && val.Substring(0,prefix.Length).ToLowerInvariant() == prefix.ToLowerInvariant())
            {
                return val.Substring(prefix.Length);
            }
            return val;
        }
            
        public static sbyte b2n(string sVal, sbyte ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToSByte(sVal, 2);
        }
        public static short b2n(string sVal, short ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToInt16(sVal, 2);
        }
        public static int b2n(string sVal, int ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToInt32(sVal, 2);
        }
        public static long b2n(string sVal, long ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToInt64(sVal, 2);
        }
        public static byte b2n(string sVal, byte ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToByte(sVal, 2);
        }
        public static ushort b2n(string sVal, ushort ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToUInt16(sVal, 2);
        }
        public static uint b2n(string sVal, uint ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToUInt32(sVal, 2);
        }
        public static ulong b2n(string sVal, ulong ignore)
        {
            sVal = SkipPrefix("0b", sVal);
            return Convert.ToUInt64(sVal, 2);
        }

        public static sbyte o2n(string sVal, sbyte ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToSByte(sVal, 8);
        }
        public static short o2n(string sVal, short ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToInt16(sVal, 8);
        }
        public static int o2n(string sVal, int ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToInt32(sVal, 8);
        }
        public static long o2n(string sVal, long ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToInt64(sVal, 8);
        }
        public static byte o2n(string sVal, byte ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToByte(sVal, 8);
        }
        public static ushort o2n(string sVal, ushort ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToUInt16(sVal, 8);
        }
        public static uint o2n(string sVal, uint ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToUInt32(sVal, 8);
        }
        public static ulong o2n(string sVal, ulong ignore)
        {
            sVal = SkipPrefix("0o", sVal);
            return Convert.ToUInt64(sVal, 8);
        }
            
        public static sbyte x2n(string sVal, sbyte ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToSByte(sVal, 16);
        }
        public static short x2n(string sVal, short ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToInt16(sVal, 16);
        }
        public static int x2n(string sVal, int ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToInt32(sVal, 16);
        }
        public static long x2n(string sVal, long ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToInt64(sVal, 16);
        }
        public static byte x2n(string sVal, byte ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToByte(sVal, 16);
        }
        public static ushort x2n(string sVal, ushort ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToUInt16(sVal, 16);
        }
        public static uint x2n(string sVal, uint ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToUInt32(sVal, 16);
        }
        public static ulong x2n(string sVal, ulong ignore)
        {
            sVal = SkipPrefix("0x", sVal);
            return Convert.ToUInt64(sVal, 16);
        }
    }
}
