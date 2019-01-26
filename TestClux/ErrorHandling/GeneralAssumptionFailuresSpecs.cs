using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Assumptions;

namespace TestClux.ErrorHandling
{
    public class GeneralAssumptionFailuresSpecs
    {
        class OptionalOverrideAndRequired
        {
            [Required]
            [Clux.Optional]
            public int conflicting;
        }

        [Fact]
        public void ShouldRejectContradictingRequiredAndOptionalDeclarations()
        {
            // conflicting option attributes
            try
            {
                Parser<OptionalOverrideAndRequired>.Parse(new []{ "--conflicting", "1" });
                Assert.False(true);
            }
            catch (AssumptionFailure  ex)
            {
                Assert.Equal("Expected explicitly optional and explicitly required (True) to be False", ex.Message);
            }
        }
     }
}
