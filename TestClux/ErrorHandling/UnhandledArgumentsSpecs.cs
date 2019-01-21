using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class UnhandledArgumentsSpecs
    {
        [StructLayout(LayoutKind.Sequential)]
        class UnhandledArgumentsArgs
        {
            [Abbreviation('p')] 
            public bool Passed;

            [Positional]
            public string Positional;
        }
        
        [Fact]
        public void RejectsEarlierUnhandledOptionsAfterPositionalPass()
        {
            try
            {
                Parser<UnhandledArgumentsArgs>.Parse(new[] {  "positional", "-p" });
                Assert.False(true);
            }
            catch (UnhandledArguments<UnhandledArgumentsArgs> ex)
            {
                Assert.Equal("-p", ex.Arguments.Single());
                Assert.Equal($"Too many arguments, or arguments out of order.  Failed to interpret: '-p'", ex.UserErrorMessage);
            }
        }
        
        [Fact]
        public void AllowsEarlierOptionsBeforePositionalPass()
        {
            Parser<UnhandledArgumentsArgs>.Parse(new[] { "--passed", "positional" });
        }
    }
}
