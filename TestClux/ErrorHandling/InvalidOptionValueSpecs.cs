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
            Value = 1
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
                Assert.Equal("asdf", ex.Value);
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
                    Assert.Equal("6/15/2009", ex.Value);
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
                Assert.Equal("2018/07/11 123333", ex.Value);
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
                Assert.Equal("2018/07/G3", ex.Value);
            }
        }
        
        public static IEnumerable<object[]> InvalidNumberValues
        {
            get
            {
                return new List<object[]>
                {
                    // overfloat
                    new object[] { "--s-byte", (1+(long)SByte.MaxValue).ToString(), "SByte" },
                    new object[] { "--short", (1+(long)Int16.MaxValue).ToString(), "Short" },
                    new object[] { "--integer", (1+(long)Int32.MaxValue).ToString(), "Integer" },
                    new object[] { "--long", ((ulong)UInt64.MaxValue).ToString(), "Long" },
                
                    // underflow
                    new object[] { "--s-byte", (-1+(long)SByte.MinValue).ToString(), "SByte" },
                    new object[] { "--short", (-1+(long)Int16.MinValue).ToString(), "Short" },
                    new object[] { "--integer", (-1+(long)Int32.MinValue).ToString(), "Integer" },
                    new object[] { "--long", "-" + ((ulong)UInt64.MaxValue).ToString(), "Long" },
                
                    // invalid signed format
                    new object[] { "--s-byte", "200.01", "SByte" },
                    new object[] { "--short", "200.01", "Short" },
                    new object[] { "--integer", "200.01", "Integer" },
                    new object[] { "--long", "200.01", "Long" },
                
                    // invalid unsigned format
                    new object[] { "--byte", "200.01", "Byte" },
                    new object[] { "--u-short", "200.01", "UShort" },
                    new object[] { "--u-integer", "200.01", "UInteger" },
                    new object[] { "--u-long", "200.01", "ULong" },
                    
                    // invalid negative input to signed
                    new object[] { "--byte", "-200", "Byte" },
                    new object[] { "--u-short", "-200", "UShort" },
                    new object[] { "--u-integer", "-200", "UInteger" },
                    new object[] { "--u-long", "-200", "ULong" },
                    
                    // unknown enum value
                    new object[] { "--enum", "2", "Enum" },
                    
                };
            }
        }
        
        [Theory]
        [MemberData(nameof(InvalidNumberValues))]
        public void DetectsInvalidNumberValues(string argName, string argValue, string member)
        {
            try
            {
                var parsed = Parser<InvalidOptionArgs>.Parse(new[] { argName, argValue });
                Assert.True(false);
            }
            catch (InvalidOptionValue<InvalidOptionArgs> ex)
            {
                Assert.Equal(member, ex.Option.Name);
                Assert.Equal(argValue, ex.Value);
            }     
        }
    }
}
