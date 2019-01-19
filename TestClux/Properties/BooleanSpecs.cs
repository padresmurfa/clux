// BooleanSpecs
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class BooleanSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        class Clopts
        {
            public bool Boolean;
            public bool? OptionalBoolean;
        }

        [Fact]
        public void SupportsBooleanOptions()
        {
            var b = parser.Parse(new[] { "--boolean=true" });
            Assert.True(b.Boolean);
            
            b = parser.Parse(new[] { "--boolean=false" });
            Assert.False(b.Boolean);
            
            b = parser.Parse(new[] { "--boolean" });
            Assert.True(b.Boolean);
        }

        public static IEnumerable<object[]> SupportsBooleanOptionSpecificFormatData
        {
            get
            {
                var now = DateTime.UtcNow;
                
                return new List<object[]>
                {
                    new object[] {  "0", false },
                    new object[] {  "f", false },
                    new object[] {  "false", false },
                    new object[] {  "n", false },
                    new object[] {  "no", false },
                    
                    new object[] {  "1", true },
                    new object[] {  "t", true },
                    new object[] {  "true", true },
                    new object[] {  "y", true },
                    new object[] {  "yes", true },
                };
            }
        }
        
        [Theory]
        [MemberData(nameof(SupportsBooleanOptionSpecificFormatData))]
        public void SupportsBooleanOptionSpecificFormat(string arg, bool expected)
        {
            var parsed = parser.Parse(new[] { $"--optional-boolean={arg}" });
            Assert.True(parsed.OptionalBoolean.HasValue);
            Assert.Equal(expected, parsed.OptionalBoolean.Value);
        }
     }
}
