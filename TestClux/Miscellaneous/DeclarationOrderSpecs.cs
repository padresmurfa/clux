// DeclarationOrderSpecs
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Miscellaneous
{
    public class DeclarationOrderSpecs
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
        public void SupportsPositionalOptions()
        {
            var clopts = parser.Parse(new[] { "the" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Null(clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Null(clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Null(clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Equal("in", clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in", "spain" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Single(clopts.Files);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in", "spain", "lies" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Equal(2, clopts.Files.Length);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.Files[1]);
            Assert.Equal("lies", clopts.LastFile);
        }

        [Fact]
        public void SupportsTrailingPositionalOptions()
        {
            var clopts = parser.Parse(new[] { "the", "rain", "in", "spain", "lies" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Equal(2, clopts.Files.Length);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.Files[1]);
            Assert.Equal("lies", clopts.LastFile);
        }

        public class Remainder
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
        }
        
        [Fact]
        public void SupportsReturningRemainder()
        {
            Parser<Remainder>.Parse(out var remainder, new string[] { "verb", "remainder", "jar" } );
            Assert.Equal(2, remainder.Length);
            
            Assert.Equal("remainder", remainder[0]);
            Assert.Equal("jar", remainder[1]);
        }

        public class RemainderError
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
            
            [Positional]
            public int i;
        }

        [Fact]
        public void SupportsReturningRemainderWhenError()
        {
            string[] remainder = null;
            try
            {
                Parser<RemainderError>.Parse(out remainder, new string[] { "verb", "remainder", "jar" } );
            }
            catch (Exception)
            {
                Assert.Equal(2, remainder.Length);
                
                Assert.Equal("remainder", remainder[0]);
                Assert.Equal("jar", remainder[1]);
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder1
        {
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg2;
            
            public string Arg3;
            
            [Positional]
            public string Arg4;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder2
        {
            public string Arg3;
            
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg2;
            
            [Positional]
            public string Arg4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder3
        {
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg3;
            
            [Positional]
            public string Arg2;
            
            [Positional]
            public string Arg4;
        }
        
        // TODO: consider enforcing a description, and having it derive from this
        //       so the creator does not have to do this manually
        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder4
        {
            public string Arg4;
            
            public string Arg3;
            
            public string Arg2;
            
            public string Arg1;
        }
        
        [Fact]
        public void DescribesArgsInCorrectOrderInHelp()
        {
            var parser1 = Parser<TestOrder1>.Create();
            
            var help = parser1.GetHelpMessage("order");
            Assert.Equal("usage: order [-a <str>] [arg1] [arg2] [arg4]", help.Split("\n").First());
            
            var parser2 = Parser<TestOrder2>.Create();
            
            help = parser2.GetHelpMessage("order");
            Assert.Equal("usage: order [-a <str>] [arg1] [arg2] [arg4]", help.Split("\n").First());
            
            var parser3 = Parser<TestOrder3>.Create();
            
            help = parser3.GetHelpMessage("order");
            Assert.Equal("usage: order [arg1] [arg3] [arg2] [arg4]", help.Split("\n").First());
            
            var parser4 = Parser<TestOrder4>.Create();
            
            help = parser4.GetHelpMessage("order");
            Assert.Equal("usage: order [--arg4 <str>] [--arg3 <str>] [--arg2 <str>] [--arg1 <str>]", help.Split("\n").First());
        }
     }
}
