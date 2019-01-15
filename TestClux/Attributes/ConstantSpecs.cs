using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class ConstantSpecs
    {
        class OptionalConstStruct
        {
            [Constant(1)]
            public int? ConstantOrDie;
            
            public bool? Detach;
        }
            
        [Fact]
        public void SupportsOptionalConstantOptions()
        {
            // TODO: switch exceptions to contain target-property instead of longOption and such.
            try
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-d" });
            }
            catch (MissingConstantOption)
            {
                Assert.False(true);
            }
            
            try
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-c", "2", "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-c", "1", "-d" });
            }
        }
        
        class RequiredConstStruct
        {
            [Constant(1)]
            [Required]
            public int? ConstantOrDie;
            
            public bool? Detach;
        }
        
        [Fact]
        public void SupportsRequiredConstantOptions()
        {
            // TODO: switch exceptions to contain target-property instead of longOption and such.
            try
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
            }
            
            try
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-c", "2", "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-c", "1", "-d" });
            }
        }
     }
}
