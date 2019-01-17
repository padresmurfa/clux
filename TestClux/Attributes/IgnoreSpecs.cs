using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class IgnoreSpecs
    {
        public class IgnoreField
        {
            [Ignore]
            public string IgnoreMe;

            [Abbreviation('I')]            
            public string IgnoreMeNot;
        }
        
        [Fact]
        public void ShouldBeAbleToMarkFieldsAsIgnored()
        {
            var parsed = Parser<IgnoreField>.Parse("-I", "ignore-me-not");
            
            try
            {
                parsed = Parser<IgnoreField>.Parse("--ignore-me", "ignore-me");
                Assert.False(true);
            }
            catch (UnknownOption<IgnoreField>)
            {
            }
        }        
     }
}
