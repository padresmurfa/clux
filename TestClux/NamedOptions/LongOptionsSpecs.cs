using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.NamedOptions
{
    public class LongOptionsSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        enum KEnumType : int
        {
            red = 0xFF0000,
            green = 0x00FF00,
            blue = 0x0000FF
        }

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
            [Positional]
            public string Pronoun { get; set; }

            [Positional]
            public string Noun { get; set; }

            [Positional]
            public string[] Files { get; set; }

            [Positional]
            public string LastFile { get; set; }

            public bool? Indicator { get; set; }

            public bool? Node;

            [Abbreviation('l')]
            public string Location { get; set; }

            [Abbreviation('L')]
            public string Lies;

            public bool? Oompfh { get; set; }

            public string The;

            public byte? KByte { get; set; }
            public ushort? KUShort { get; set; }
            public uint? KUInt { get; set; }
            public ulong? KULong { get; set; }

            public sbyte? KSByte { get; set; }
            public short? KShort { get; set; }
            public int? KInt { get; set; }
            public long? KLong { get; set; }

            public decimal? KDecimal { get; set; }
            public float? KFloat { get; set; }
            public double? KDouble { get; set; }

            public KEnumType? KEnum;

            public char? KChar;

            public bool? KTheRainInSpainLiesMainlyOnThePlains;

            public int? KNullableInt { get; set; }

            public int[] KInts { get; set; }

            public DateTime? KDateTime { get; set;}

            public string KString;

            public string[] KMultiple { get; set; }
        }

        [Fact]
        public void SupportsKeyValOptions()
        {
            var clopts = parser.Parse(new[] { "--the=plains" });

            Assert.Equal("plains", clopts.The);
        }

        [Fact]
        public void SupportsLongOptions()
        {
            var clopts = parser.Parse(new[] { "--the", "mainly" });

            Assert.Equal("mainly", clopts.The);
        }

        [Fact]
        public void SupportsLongOptionsWithParams()
        {
            var clopts = parser.Parse(new[] { "--the", "rain" });

            Assert.Equal("rain", clopts.The);
        }

        [Fact]
        public void SupportsKebabCase()
        {
            var clopts = parser.Parse(new[] { "--k-the-rain-in-spain-lies-mainly-on-the-plains" });

            Assert.True(clopts.KTheRainInSpainLiesMainlyOnThePlains);
        }

        [Fact]
        public void DoesNotSupportDuplicateOptionsByDefault()
        {
            try
            {
                parser.Parse(new[] { "--k-string", "a", "--k-string", "b" });
                Assert.False(true);
            }
            catch (DuplicateOption)
            {
            }
        }

        [Fact]
        public void SupportsMultipleInstancesOfSameOption()
        {
            var clopts = parser.Parse(new[] { "--k-multiple", "a", "--k-multiple", "b" });

            Assert.Equal(2, clopts.KMultiple.Length);
            Assert.Equal("a", clopts.KMultiple[0]);
            Assert.Equal("b", clopts.KMultiple[1]);
        }
     }
}
