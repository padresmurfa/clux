using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class PropertiesAndFieldsSpecs
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
            public bool? Indicator { get; set; }

            public bool? Node;
        }

        [Fact]
        public void SupportsProperties()
        {
            var clopts = parser.Parse(new[] { "--indicator" });

            Assert.True(clopts.Indicator);
        }

        [Fact]
        public void SupportsFields()
        {
            var clopts = parser.Parse(new[] { "--node" });

            Assert.True(clopts.Node);
        }
        
        public class IgnoreROWO
        {
            public string IgnoreMe { get; private set; }

            public string IgnoreMeToo { private get; set; }

            [Abbreviation('I')]            
            public string IgnoreMeNot;
        }

        [Fact]
        public void ShouldIgnoreNonReadableWritableProperties()
        {
            var parsed = Parser<IgnoreROWO>.Parse("-I", "ignore-me-not");
            
            try
            {
                parsed = Parser<IgnoreROWO>.Parse("--ignore-me", "ignore-me");
                Assert.False(true);
            }
            catch (UnknownOption)
            {
            }
            
            try
            {
                parsed = Parser<IgnoreROWO>.Parse("--ignore-me-too", "ignore-me-too");
                Assert.False(true);
            }
            catch (UnknownOption)
            {
            }
        }                
     }
}
