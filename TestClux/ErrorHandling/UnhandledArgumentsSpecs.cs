using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.NamedOptions
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
            }
        }
        
        [Fact]
        public void AllowsEarlierOptionsBeforePositionalPass()
        {
            Parser<UnhandledArgumentsArgs>.Parse(new[] { "--passed", "positional" });
        }
    }
}
