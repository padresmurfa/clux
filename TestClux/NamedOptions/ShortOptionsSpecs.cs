using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.NamedOptions
{
    public class ShortOptionsSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
            public bool? Indicator { get; set; }

            public bool? Node;

            public bool? Oompfh { get; set; }

            public string The;
        }

        [Fact]
        public void SupportsShortOptions()
        {
            var clopts = parser.Parse(new[] { "-o" });

            Assert.True(clopts.Oompfh);
        }

        [Fact]
        public void SupportsMergedShortOptions()
        {
            var clopts = parser.Parse(new[] { "-in" });

            Assert.True(clopts.Indicator);
            Assert.True(clopts.Node);
        }

        [Fact]
        public void SupportsShortOptionsWithParams()
        {
            var clopts = parser.Parse(new[] { "-t", "rain" });

            Assert.Equal("rain", clopts.The);
        }
        
        class NamedBoolsTest
        {
            [Positional]
            public bool foo;
            
            public bool notNullable;
            
            public bool? nullable;
        }
        
        [Fact]
        public void NamedBoolsShouldBeOptionalByDefault()
        {
            var parsed = Parser<NamedBoolsTest>.Parse(new []{ "true"});
            
            Assert.True(parsed.foo);
            Assert.False(parsed.notNullable);
            Assert.Null(parsed.nullable);
        }
     }
}
