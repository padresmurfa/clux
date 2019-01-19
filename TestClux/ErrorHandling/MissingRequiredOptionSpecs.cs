using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class MissingRequiredOptionSpecs
    {
        class MissingRequiredOptionArgs
        {
            [Required]
            public string Required;
            
            public string NotRequired;
        }

        [Fact]
        public void ShouldBeHappyWithPresentRequiredOption()
        {
            Parser<MissingRequiredOptionArgs>.Parse("--required", "happy");
        }
        
        [Fact]
        public void ShouldBeUnhappyAboutMissingRequiredOption()
        {
            try
            {
                Parser<MissingRequiredOptionArgs>.Parse("--not-required", "unhappy");
                Assert.True(false);
            }
            catch (MissingRequiredOption<MissingRequiredOptionArgs> ex)
            {
                Assert.Equal("Required", ex.Option.Name);
            }
        }
    }
}
