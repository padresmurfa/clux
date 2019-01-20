using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class AmbiguousOptionSpecs
    {
        class AmbiguousShortOption
        {
            [Abbreviation('a')]
            public bool Argument;
            
            [Abbreviation('a')]
            public bool Argh;
        }

        [Fact]
        public void RejectsExplicitAmbiguousOptionDeclarations()
        {
            try
            {
                Parser<AmbiguousShortOption>.Parse(new[] { "-a" });
                Assert.True(false);
            }
            catch (AmbiguousOption<AmbiguousShortOption> o)
            {
                Assert.Equal( 2, o.Options.Count() );
                Assert.True( o.Options.Any(x => x.Name == "Argument") );
                Assert.True( o.Options.Any(x => x.Name == "Argh") );
                
                Assert.Equal("Ambiguous argument declaration. '-a' could refer to any of the following: 'Argh' or 'Argument'", o.UserErrorMessage);
            }
        }
        
        class IgnoredShortOption
        {
            public bool Argument;
            
            public bool Argh;
        }

        [Fact]
        public void RejectsImplicitAmbiguousOptions()
        {
            try
            {
                Parser<IgnoredShortOption>.Parse(new[] { "-a" });
                Assert.True(false);
            }
            catch (AmbiguousOption<IgnoredShortOption> o)
            {
                Assert.Equal("Ambiguous argument. '-a' could refer to any of the following: 'Argh' or 'Argument'", o.UserErrorMessage);
            }
        }
        
        class ExplicitlyDisambiguitedOptions
        {
            [Abbreviation('a')]
            public bool Argument;
            
            public bool Argh;
        }

        [Fact]
        public void AcceptsExplicitlyDisambiguitedOptions()
        {
            var parsed = Parser<ExplicitlyDisambiguitedOptions>.Parse(new[] { "-a" });
            
            Assert.True(parsed.Argument);
            Assert.False(parsed.Argh);
        }
    }
}
