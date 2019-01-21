using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class UnknownMergedShortOptionSpecs
    {
        class Args
        {
            public bool One;
            public bool Two;
        }
        
        [Fact]
        public void ShouldUnwrapSuccessfully()
        {
            var parsed = Parser<Args>.Parse("-ot");
            Assert.True(parsed.One);
            Assert.True(parsed.Two);
        }

        [Fact]
        public void ShouldRejectUnwrappingUnknownOption()
        {
            try
            {
                Parser<Args>.Parse("-otn");
                Assert.True(false);                
            }
            catch (UnknownMergedShortOption<Args> ex)
            {
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("-otn", ex.Remainder.First());
                Assert.Equal("otn", ex.UnmergedOption);
                Assert.Equal("n", ex.OptionName);
                
                Assert.Equal("Unknown short option '-n'.  Could not separate short options from '-otn'", ex.UserErrorMessage);
            }
        }
    }
}
