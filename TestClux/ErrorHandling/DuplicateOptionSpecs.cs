using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class DuplicateOptionSpecs
    {
        [StructLayout(LayoutKind.Sequential)]
        class DuplicateOptionArgs
        {
            [Usage("")]
            public bool Flag;
            
            [Usage("")]
            public string Option;
        }

        [Fact]
        public void DoesNotRejectNonDuplicateOptions()
        {
            Parser<DuplicateOptionArgs>.Parse(new[] { "-f" });
            Parser<DuplicateOptionArgs>.Parse(new[] { "-o", "option" });
        }
        
        [Fact]
        public void RejectsDuplicateFlag()
        {
            try
            {
                Parser<DuplicateOptionArgs>.Parse(new[] { "-f", "-f" });
                Assert.False(true);
            }
            catch (DuplicateOption<DuplicateOptionArgs> ex)
            {
                Assert.Equal("Flag", ex.Option.Name);
            }
        }
        
        [Fact]
        public void RejectsDuplicateShortOption()
        {
            try
            {
                Parser<DuplicateOptionArgs>.Parse(new[] { "-o", "option", "-o", "option" });
                Assert.False(true);
            }
            catch (DuplicateOption<DuplicateOptionArgs> ex)
            {
                Assert.Equal("Option", ex.Option.Name);
            }
        }
        
        [Fact]
        public void RejectsDuplicateLongOption()
        {
            try
            {
                Parser<DuplicateOptionArgs>.Parse(new[] { "--option", "option", "--option", "option" });
                Assert.False(true);
            }
            catch (DuplicateOption<DuplicateOptionArgs> ex)
            {
                Assert.Equal("Option", ex.Option.Name);
            }
        }

        [Fact]
        public void RejectsDuplicateMixedOption()
        {
            try
            {
                Parser<DuplicateOptionArgs>.Parse(new[] { "-o", "option", "--option", "option" });
                Assert.False(true);
            }
            catch (DuplicateOption<DuplicateOptionArgs> ex)
            {
                Assert.Equal("Option", ex.Option.Name);
            }
        }
        
        [Fact]
        public void RejectsDuplicateLongKeyValOption()
        {
            try
            {
                Parser<DuplicateOptionArgs>.Parse(new[] { "--option=option", "--option=option" });
                Assert.False(true);
            }
            catch (DuplicateOption<DuplicateOptionArgs> ex)
            {
                Assert.Equal("Option", ex.Option.Name);
            }
        }

    }
}
