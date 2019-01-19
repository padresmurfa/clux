// MissingConstantOptionSpecs
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class IncorrectConstantOptionSpecs
    {
        class IncorrectConstantOptionArgs
        {
            [Constant(42)]
            public int Constant;
            
            [Constant(42)]
            [Clux.Optional]
            public int OptionalConstant;
        }

        [Fact]
        public void ShouldBeHappyWithCorrectAnswer()
        {
            Parser<IncorrectConstantOptionArgs>.Parse("-c","42");
        }
        
        [Fact]
        public void ShouldBeUnhappyAboutIncorrectAnswer()
        {
            try
            {
                Parser<IncorrectConstantOptionArgs>.Parse("-c", "43");
                Assert.True(false);
            }
            catch (IncorrectConstantOption<IncorrectConstantOptionArgs> ex)
            {
                Assert.Equal("Constant", ex.Option.Name);
                Assert.Equal(43, ex.IncorrectValue);
            }
        }
        
        [Fact]
        public void ShouldBeUnhappyAboutPresentButIncorrectOptionalConstant()
        {
            try
            {
                Parser<IncorrectConstantOptionArgs>.Parse("-o", "43");
                Assert.True(false);
            }
            catch (IncorrectConstantOption<IncorrectConstantOptionArgs> ex)
            {
                Assert.Equal("OptionalConstant", ex.Option.Name);
                Assert.Equal(43, ex.IncorrectValue);
            }
        }
    }
}
