using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class ErrorHelpSpecs
    {
        class Arg
        {
            [Abbreviation('a')]
            public bool Argument;
            
            [Abbreviation('a')]
            public bool Argh;
        }

        [Fact]
        public void ProvidesHelpMessageFromParserExceptions()
        {
            try
            {
                Parser<Arg>.Parse(new[] { "-a" });
                Assert.True(false);
            }
            catch (AmbiguousOption<Arg> o)
            {
                Assert.False(string.IsNullOrEmpty(o.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ProvidesHelpMessageFromAllParserExceptions()
        {
            throw new NotImplementedException(); 
        }
    }
}
