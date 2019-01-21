using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class NonBoolMergedShortOptionSpecs
    {
        class Args
        {
            public bool One;
            public bool Two;
            
            public string A;
            public string B;
        }
        
        [Fact]
        public void ShouldUnwrapSuccessfully()
        {
            var parsed = Parser<Args>.Parse("-ot");
            Assert.True(parsed.One);
            Assert.True(parsed.Two);
        }

        [Fact]
        public void ShouldRejectUnwrappingNonBooleanOptionMixedWithBooleanOptions()
        {
            try
            {
                Parser<Args>.Parse("-oa");
                Assert.True(false);                
            }
            catch (NonBoolMergedShortOption<Args> ex)
            {
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("-oa", ex.Remainder.First());
                Assert.Equal("oa", ex.UnmergedOption);
                Assert.Equal("a", ex.OptionName);
                Assert.Equal("'-oa' contains the non-boolean option 'a', which requires a value and cannot be merged with other short options", ex.UserErrorMessage);
            }
        }
        
        [Fact]
        public void ShouldRejectUnwrappingMultipleNonBooleanOptions()
        {
            try
            {
                Parser<Args>.Parse("-ab");
                Assert.True(false);                
            }
            catch (NonBoolMergedShortOption<Args> ex)
            {
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("-ab", ex.Remainder.First());
                Assert.Equal("ab", ex.UnmergedOption);
                Assert.Equal("a", ex.OptionName);
                Assert.Equal("'-ab' contains the non-boolean option 'a', which requires a value and cannot be merged with other short options", ex.UserErrorMessage);
            }
        }
    }
}
