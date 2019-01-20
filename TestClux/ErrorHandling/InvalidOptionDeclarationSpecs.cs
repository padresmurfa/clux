using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class InvalidOptionDeclarationSpecs
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
            catch (InvalidOptionDeclaration<OptionalOverrideAndRequired> ex)
            {
                Assert.Equal("conflicting", ex.Option.Name);
                
                Assert.Equal("Conflicting option declaration. 'conflicting' was explicitly declared to be both Optional and Required", ex.UserErrorMessage);
            }
        }
     }
}
