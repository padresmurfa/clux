using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.NamedOptions
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
        public void RejectsExplicitAmbiguousOptions()
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
            catch (UnknownOption<IgnoredShortOption> o)
            {
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
