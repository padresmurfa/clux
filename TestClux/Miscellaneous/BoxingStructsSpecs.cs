using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Miscellaneous
{
    public class BoxingStructsSpecs
    {
        struct BoxingUnboxingTest
        {
            [Positional]
            public bool foo;
            
            public bool notNullable;
            
            public bool? nullable;
        }

        [Fact]
        public void ShouldBeImmuneToBoxingUnboxingErrors()
        {
            // see Chris Shain's descussion here: https://stackoverflow.com/questions/9694404/propertyinfo-setvalue-not-working-but-no-errors
            
            var parsed = Parser<BoxingUnboxingTest>.Parse(new []{ "true"});
            
            Assert.True(parsed.foo);
            Assert.False(parsed.notNullable);
            Assert.Null(parsed.nullable);
        }
     }
}
