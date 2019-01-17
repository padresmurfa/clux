using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class ArraySpecs
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
            public string[] Files { get; set; }

            [Positional]
            public string LastFile { get; set; }

            public int[] KInts { get; set; }

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
        public void SupportsArrayOptions()
        {
            var clopts = parser.Parse(new[] { "--k-ints", "0", "1", "2", "3" });

            Assert.Equal(4, clopts.KInts.Length);

            Assert.Equal(0, clopts.KInts[0]);
            Assert.Equal(1, clopts.KInts[1]);
            Assert.Equal(2, clopts.KInts[2]);
            Assert.Equal(3, clopts.KInts[3]);
        }

        [Fact]
        public void DoesNotSupportDuplicateOptionsByDefault()
        {
            try
            {
                parser.Parse(new[] { "--k-string", "a", "--k-string", "b" });
                Assert.False(true);
            }
            catch (DuplicateOption<Clopts>)
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
