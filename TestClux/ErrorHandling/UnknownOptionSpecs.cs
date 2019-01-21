using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class UnknownOptionSpecs
    {
        class UnknownOptionArgs
        {
            public string KnownOption;
        }

        [Fact]
        public void ShouldBeHappyWithAKnownOption()
        {
            Parser<UnknownOptionArgs>.Parse("--known-option", "ko");
        }
        
        [Fact]
        public void ShouldNotBeHappyWithAnUnknownLongOption()
        {
            try
            {
                Parser<UnknownOptionArgs>.Parse("--unknown-option", "uko");
                Assert.True(false);
            }
            catch (UnknownLongOption<UnknownOptionArgs> ex)
            {
                Assert.Equal("unknown-option", ex.OptionName);
                Assert.Equal("Unknown long option: '--unknown-option'", ex.UserErrorMessage);
            }
        }
        
        [Fact]
        public void ShouldNotBeHappyWithAnUnknownShortOption()
        {
            try
            {
                Parser<UnknownOptionArgs>.Parse("-u", "uko");
                Assert.True(false);
            }
            catch (UnknownShortOption<UnknownOptionArgs> ex)
            {
                Assert.Equal("u", ex.OptionName);
                Assert.Equal("Unknown short option: '-u'", ex.UserErrorMessage);
            }
        }
    }
}
