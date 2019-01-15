using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class BasicTypesSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
            [Positional]
            public string Pronoun { get; set; }

            [Positional]
            public string Noun { get; set; }

            [Positional]
            public string LastFile { get; set; }

            public bool? Indicator { get; set; }

            public bool? Node;

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

            public char? KChar;

            public bool? KTheRainInSpainLiesMainlyOnThePlains;

            public int? KNullableInt { get; set; }

            public string KString;
        }
        
        [Fact]
        public void SupportsBooleanOptions()
        {
            var clopts = parser.Parse(new[] { "--indicator" });

            Assert.True(clopts.Indicator);
        }

        [Fact]
        public void SupportsStringOptions()
        {
            var clopts = parser.Parse(new[] { "--the", "mainly" });

            Assert.Equal("mainly", clopts.The);
        }

        [Fact]
        public void SupportsSbyteIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-s-byte", "33" });

            Assert.Equal((sbyte)33, clopts.KSByte);
        }

        [Fact]
        public void SupportsShortIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-short", "333" });

            Assert.Equal((short)333, clopts.KShort);
        }

        [Fact]
        public void SupportsIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "333" });

            Assert.Equal(333, clopts.KInt);
        }

        [Fact]
        public void SupportsLongIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-long", "333" });

            Assert.Equal(333, clopts.KLong);
        }

        [Fact]
        public void SupportsByteIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-byte", "33" });

            Assert.Equal((byte)33, clopts.KByte);
        }

        [Fact]
        public void SupportsUShortIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-short", "333" });

            Assert.Equal((ushort)333, clopts.KUShort);
        }

        [Fact]
        public void SupportsUIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-int", "333" });

            Assert.Equal((uint)333, clopts.KUInt);
        }

        [Fact]
        public void SupportsULongIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-long", "333" });

            Assert.Equal((ulong)333, clopts.KULong);
        }

        [Fact]
        public void SupportsCharOptions()
        {
            var clopts = parser.Parse(new[] { "--k-char", "a" });

            Assert.Equal('a', clopts.KChar);
        }

        [Fact]
        public void SupportsNullableOptions()
        {
            var clopts = parser.Parse(new[] { "--k-nullable-int", "333" });

            Assert.Equal(333, clopts.KNullableInt);
        }

        [Fact]
        public void SupportsDecimalNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-decimal", "1.303" });

            Assert.Equal(1.303m, clopts.KDecimal);
        }

        [Fact]
        public void SupportsFloatNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-float", "1.303" });

            Assert.Equal(1.303f, clopts.KFloat);
        }

        [Fact]
        public void SupportsDoubleNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-double", "1.303" });

            Assert.Equal(1.303, clopts.KDouble);
        }
     }
}
