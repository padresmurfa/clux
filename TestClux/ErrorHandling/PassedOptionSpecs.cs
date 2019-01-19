using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class PassedOptionSpecs
    {
        class PassedOptionArgs
        {
            [Required]
            public string Required;
            
            public string NotRequired;
        }

        [Fact(Skip="Seems unreachable atm, need to investigate repro cases better")]
        public void ShouldBeHappyWithPresentRequiredOption()
        {
        }
    }
}
