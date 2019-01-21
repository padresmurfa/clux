using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class MissingOptionValueSpecs
    {
        class MissingOptionValueArgs
        {
            public string LongString;
            
            public bool LongBool;
        }
        
        [Fact]
        public void ShouldBeHappyWithPresentLongOptionValue()
        {
            Parser<MissingOptionValueArgs>.Parse("--long-string", "happy");
        }
        
        [Fact]
        public void ShouldBeHappyWithMissingLongOptionBooleanValue()
        {
            Parser<MissingOptionValueArgs>.Parse("--long-bool");
        }
        
        
        [Fact]
        public void ShouldBeUnhappyWithMissingLongOptionStringValue()
        {
            try
            {
                Parser<MissingOptionValueArgs>.Parse("--long-string");
                Assert.True(false);
            }
            catch (MissingOptionValue<MissingOptionValueArgs> ex)
            {
                Assert.Equal("LongString", ex.Option.Name);
                Assert.Equal("Missing option value. 'LongString' must be provided with a value", ex.UserErrorMessage);
            }
        }
    }
}
