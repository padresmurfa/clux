using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class UnexpectedPositionalOptionSpecs
    {
        class UnexpectedPositionalOptionArgs
        {
            [Positional]
            [Required]
            public string First;
        }

        [Fact]
        public void ShouldBeHappyWithNormalPositionalOptions()
        {
            Parser<UnexpectedPositionalOptionArgs>.Parse("first");
        }
        
        [Fact]
        public void ShouldBeUnhappyWithExtraPositionalOptions()
        {
            try
            {
                Parser<UnexpectedPositionalOptionArgs>.Parse("first");
            }
            catch (UnexpectedPositionalOption<UnexpectedPositionalOptionArgs> ex)
            {
            
            }
        }
    }
}
