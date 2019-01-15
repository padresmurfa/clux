using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class IntegerBaseBinaryOctalHexSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
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

            public int[] KInts { get; set; }
        }


        [Fact]
        public void SupportsHexadecimalOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0x333" });

            Assert.Equal((int)0x333, clopts.KInt);
        }

        [Fact]
        public void SupportsBinaryOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0b111000" });

            Assert.Equal((int)0b111000, clopts.KInt);
        }

        [Fact]
        public void SupportsOctalOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0o111000" });

            Assert.Equal(S2N.o2n("0o111000", default(int)), clopts.KInt);
        }

        [Fact]
        public void SupportsBaseSpecifierCaseInsensitivelyForIntegerOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0B111" });
            Assert.Equal((int)0b111, clopts.KInt);

            clopts = parser.Parse(new[] { "--k-int", "0O111" });

            Assert.Equal(S2N.o2n("0o111", default(int)), clopts.KInt);

            clopts = parser.Parse(new[] { "--k-int", "0X111" });
            Assert.Equal((int)0x111, clopts.KInt);
        }

        [Fact]
        public void SupportsAllBaseSpecifiersForAllIntegerOptions()
        {
            var binary = parser.Parse(new[] {
                "--k-s-byte", "0b111",
                "--k-short", "0b111",
                "--k-int", "0b111",
                "--k-long", "0b111",
                "--k-byte", "0b111",
                "--k-u-short", "0b111",
                "--k-u-int", "0b111",
                "--k-u-long", "0b111"
            });
            Assert.Equal((sbyte)0b111, binary.KSByte);
            Assert.Equal((short)0b111, binary.KShort);
            Assert.Equal((int)0b111, binary.KInt);
            Assert.Equal((long)0b111, binary.KLong);

            Assert.Equal(((byte)0b111), binary.KByte);
            Assert.Equal((ushort)0b111, binary.KUShort);
            Assert.Equal((uint)0b111, binary.KUInt);
            Assert.Equal((ulong)0b111, binary.KULong);

            var oct = parser.Parse(new[] {
                "--k-s-byte", "0o111",
                "--k-short", "0o111",
                "--k-int", "0o111",
                "--k-long", "0o111",
                "--k-byte", "0o111",
                "--k-u-short", "0o111",
                "--k-u-int", "0o111",
                "--k-u-long", "0o111"
            });
            Assert.Equal(S2N.o2n("0o111", default(sbyte)), oct.KSByte);
            Assert.Equal(S2N.o2n("0o111", default(short)), oct.KShort);
            Assert.Equal(S2N.o2n("0o111", default(int)), oct.KInt);
            Assert.Equal(S2N.o2n("0o111", default(long)), oct.KLong);

            Assert.Equal(S2N.o2n("0o111", default(byte)), oct.KByte);
            Assert.Equal(S2N.o2n("0o111", default(ushort)), oct.KUShort);
            Assert.Equal(S2N.o2n("0o111", default(uint)), oct.KUInt);
            Assert.Equal(S2N.o2n("0o111", default(ulong)), oct.KULong);

            var hex = parser.Parse(new[] {
                "--k-s-byte", "0x11",
                "--k-short", "0x111",
                "--k-int", "0x111",
                "--k-long", "0x111",
                "--k-byte", "0x11",
                "--k-u-short", "0x111",
                "--k-u-int", "0x111",
                "--k-u-long", "0x111"
            });

            Assert.Equal((sbyte)0x11, hex.KSByte);
            Assert.Equal((short)0x111, hex.KShort);
            Assert.Equal((int)0x111, hex.KInt);
            Assert.Equal((long)0x111, hex.KLong);

            Assert.Equal(((byte)0x11), hex.KByte);
            Assert.Equal((ushort)0x111, hex.KUShort);
            Assert.Equal((uint)0x111, hex.KUInt);
            Assert.Equal((ulong)0x111, hex.KULong);
        }
     }
}
