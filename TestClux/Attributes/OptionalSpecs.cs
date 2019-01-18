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
