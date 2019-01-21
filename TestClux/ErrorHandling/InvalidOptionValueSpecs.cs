using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class InvalidOptionValueSpecs
    {
        enum Bla
        {
            Value = 1,
            TheAnswer = 42
        }
        
        [StructLayout(LayoutKind.Sequential)]
        class InvalidOptionArgs
        {
            [Clux.Optional]
            public bool Boolean;
            
            public DateTime? DateTime;

            [Clux.Optional]
            public sbyte SByte;

            [Clux.Optional]
            public short Short;

            [Clux.Optional]
            public int Integer;

            [Clux.Optional]
            public long Long;
            
            [Clux.Optional]
            public byte Byte;

            [Clux.Optional]
            public ushort UShort;

            [Clux.Optional]
            public uint UInteger;

            [Clux.Optional]
            public ulong ULong;
            
            [Clux.Optional]
            public Bla Enum;
            
            [Clux.Optional]
            public float Float;
            
            [Clux.Optional]
            public double Double;
            
            [Clux.Optional]
            public decimal Decimal;
        }

        [Fact]
        public void RejectsInvalidBooleans()
        {
            try
            {
                Parser<InvalidOptionArgs>.Parse(new[] { "--boolean=asdf" });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal("Boolean", ex.Option.Name);
                Assert.Equal("asdf", ex.InvalidValue);
                Assert.Equal("Invalid flag. 'Boolean' cannot accept the value 'asdf'.  It can only accept true or false values. (i.e. 1/0, true/false, t/f, yes/no or y/n)", ex.UserErrorMessage);
            }
        }
        
        [Fact(Skip = "Don't actually know of any ambiguous formats")]
        public void RejectsAmbiguousDateTimeFormats()
        {
            var previous = System.Threading.Thread.CurrentThread.CurrentCulture;
            var previousUI = System.Threading.Thread.CurrentThread.CurrentUICulture;
            
            var culture = new System.Globalization. CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            
            try
            {
                try
                {
                    Parser<InvalidOptionArgs>.Parse(new[] { "--date-time", " 6/15/2009" });
                    Assert.True(false);
                }
                catch (InvalidOptionValue<InvalidOptionArgs> ex)
                {
                    Assert.Equal("DateTime", ex.Option.Name);
                    Assert.Equal("6/15/2009", ex.InvalidValue);
                    Assert.Equal("todo", ex.UserErrorMessage);
                }
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = previous;
                System.Threading.Thread.CurrentThread.CurrentUICulture = previousUI;
            }
        }
        
        [Fact]
        public void RejectsUnknownDateTimeFormats()
        {
            try
            {
                Parser<InvalidOptionArgs>.Parse(new[] { "--date-time", "2018/07/11 123333" });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal("DateTime", ex.Option.Name);
                Assert.Equal("2018/07/11 123333", ex.InvalidValue);
                Assert.Equal("Invalid date/time. 'DateTime' cannot accept the value '2018/07/11 123333'.  It can only accept valid date/time formats according to the current locale (region) settings and a few other standard and convenient formats.  See https://github.com/padresmurfa/clux for more details", ex.UserErrorMessage);
            }
        }
        
        [Fact]
        public void RejectsInvalidValueDateTimeFormats()
        {
            try
            {
                Parser<InvalidOptionArgs>.Parse(new[] { "--date-time", "2018/07/G3" });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal("DateTime", ex.Option.Name);
                Assert.Equal("2018/07/G3", ex.InvalidValue);
                Assert.Equal("Invalid date/time. 'DateTime' cannot accept the value '2018/07/G3'.  It can only accept valid date/time formats according to the current locale (region) settings and a few other standard and convenient formats.  See https://github.com/padresmurfa/clux for more details", ex.UserErrorMessage);
            }
        }
        
        public static IEnumerable<object[]> InvalidIntegerValues
        {
            get
            {
                return new List<object[]>
                {
                    // overfloat
                    new object[] { "--s-byte", (1+(long)SByte.MaxValue).ToString(), "SByte", "8-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--short", (1+(long)Int16.MaxValue).ToString(), "Short", "16-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--integer", (1+(long)Int32.MaxValue).ToString(), "Integer", "32-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--long", ((ulong)UInt64.MaxValue).ToString(), "Long", "64-bit signed integers (optionally specified in base 2, 8, or 16)" },
                
                    // underflow
                    new object[] { "--s-byte", (-1+(long)SByte.MinValue).ToString(), "SByte", "8-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--short", (-1+(long)Int16.MinValue).ToString(), "Short", "16-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--integer", (-1+(long)Int32.MinValue).ToString(), "Integer", "32-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--long", "-" + ((ulong)UInt64.MaxValue).ToString(), "Long", "64-bit signed integers (optionally specified in base 2, 8, or 16)" },
                
                    // invalid signed format
                    new object[] { "--s-byte", "200.01", "SByte", "8-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--short", "200.01", "Short", "16-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--integer", "200.01", "Integer", "32-bit signed integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--long", "200.01", "Long", "64-bit signed integers (optionally specified in base 2, 8, or 16)" },
                
                    // invalid unsigned format
                    new object[] { "--byte", "200.01", "Byte", "8-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-short", "200.01", "UShort", "16-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-integer", "200.01", "UInteger", "32-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-long", "200.01", "ULong", "64-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    
                    // invalid negative input to signed
                    new object[] { "--byte", "-200", "Byte", "8-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-short", "-200", "UShort", "16-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-integer", "-200", "UInteger", "32-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--u-long", "-200", "ULong", "64-bit unsigned integers (optionally specified in base 2, 8, or 16)" },
                };
            }
        }
        
        [Theory]
        [MemberData(nameof(InvalidIntegerValues))]
        public void DetectsInvalidIntegerValues(string argName, string argValue, string member, string allowed)
        {
            try
            {
                var parsed = Parser<InvalidOptionArgs>.Parse(new[] { argName, argValue });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal(member, ex.Option.Name);
                Assert.Equal(argValue, ex.InvalidValue);
                
                Assert.Equal($"Invalid number. '{ex.OptionName}' cannot accept the value '{ex.InvalidValue}'.  It can only accept valid {allowed}.  See https://github.com/padresmurfa/clux for more details", ex.UserErrorMessage);
            }     
        }
        
        
        [Fact]
        public void DetectsInvalidEnumValues()
        {
            try
            {
                var parsed = Parser<InvalidOptionArgs>.Parse(new[] { "--enum", "2" });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal("Enum", ex.Option.Name);
                Assert.Equal("2", ex.InvalidValue);
                
                var allowed = "'value' (1) or 'theanswer' (42)";
                Assert.Equal($"Invalid value. '{ex.OptionName}' cannot accept the value '{ex.InvalidValue}'.  It can only accept values from the following list: {allowed}", ex.UserErrorMessage);
            }     
        }
        
        public static IEnumerable<object[]> InvalidFractionalValues
        {
            get
            {
                return new List<object[]>
                {
                    // TODO: various invalid forms
                    new object[] { "--decimal", "1" + (Decimal.MaxValue).ToString(), "Decimal", "decimal floating point or integral numbers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--float", "1" + (float.MaxValue).ToString(), "Float", "single-precision floating point or integral numbers (optionally specified in base 2, 8, or 16)" },
                    new object[] { "--double", "1" + (double.MaxValue).ToString(), "Double", "double-precision floating point or integral numbers (optionally specified in base 2, 8, or 16)" },

                };
            }
        }
        
        [Theory]
        [MemberData(nameof(InvalidFractionalValues))]
        public void DetectsInvalidFractionalValues(string argName, string argValue, string member, string allowed)
        {
            try
            {
                var parsed = Parser<InvalidOptionArgs>.Parse(new[] { argName, argValue });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal(member, ex.Option.Name);
                Assert.Equal(argValue, ex.InvalidValue);
                
                Assert.Equal($"Invalid number. '{ex.OptionName}' cannot accept the value '{ex.InvalidValue}'.  It can only accept valid {allowed}.  See https://github.com/padresmurfa/clux for more details", ex.UserErrorMessage);
            }     
        }
    }
}
