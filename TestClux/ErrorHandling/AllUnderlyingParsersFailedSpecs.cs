using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class AllUnderlyingParsersFailedSpecs
    {
        class AllUnderlyingParsersFailedArgs
        {
            [Required]
            public string Required;
            
            public string NotRequired;
        }

        [Fact(Skip="TODO - test parser unions errors")]
        public void ShouldBeHappyWithPresentRequiredOption()
        {
        }
    }
}
