using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class MissingConstantOptionSpecs
    {
        class MissingConstantOptionArgs
        {
            [Constant(42)]
            public int Constant;
            
            public bool Bork;
            
            [Constant(42)]
            [Clux.Optional]
            public int OptionalConstant;
        }

        [Fact]
        public void ShouldBeHappyWithCorrectAnswer()
        {
            Parser<MissingConstantOptionArgs>.Parse("-c","42");
        }
        
        [Fact]
        public void ShouldBeUnhappyAboutMissingRequiredConstant()
        {
            try
            {
                Parser<MissingConstantOptionArgs>.Parse("-b");
                Assert.True(false);
            }
            catch (MissingConstantOption<MissingConstantOptionArgs> ex)
            {
                Assert.Equal("Constant", ex.Option.Name);
                Assert.Equal("Missing constant value. 'Constant' must have the value '42'", ex.UserErrorMessage);
            }
        }
        
        [Fact]
        public void ShouldBeHappyWithMissingOptionalAnswer()
        {
            Parser<MissingConstantOptionArgs>.Parse("-c","42");
        }
    }
}
