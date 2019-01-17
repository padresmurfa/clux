using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class OptionalSpecs
    {
        class OptionalOverrideTests
        {
            [Clux.Optional]            
            public int optionalNotNullable;
            
            [Clux.Optional]            
            public int? optionalNullable;
        }
        
        class OptionalOverrideAndRequired
        {
            [Required]
            [Clux.Optional]
            public int conflicting;
        }

        [Fact]
        public void ShouldHaveOptionalAttributeThatIsInverseOfRequired()
        {
            // this should work as a base
            var parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--optional-not-nullable", "1",
                "--optional-nullable", "2"
            });
            Assert.Equal(1, parsed.optionalNotNullable);
            Assert.Equal(2, parsed.optionalNullable);

            // confliction option attributes
            try
            {
                Parser<OptionalOverrideAndRequired>.Parse(new []{ "--conflicting", "1" });
                Assert.False(true);
            }
            catch (InvalidOptionDeclaration<OptionalOverrideAndRequired>)
            {
            }
            
            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                //"--optional-not-nullable", "1",
                "--optional-nullable", "2"
            });
            Assert.Equal(0, parsed.optionalNotNullable);
            Assert.Equal(2, parsed.optionalNullable);

            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--optional-not-nullable", "1",
                //"--optional-nullable", "2"
            });
            Assert.Equal(1, parsed.optionalNotNullable);
            Assert.Null(parsed.optionalNullable);
        }
     }
}
