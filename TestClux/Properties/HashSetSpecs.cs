using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class HashSetSpecs
    {
        class HashSetTest
        {
            public HashSet<string> hashset;
        }
        
        [Fact]
        public void ShouldTreatHashSetLikeArray()
        {
            var parsed = Parser<HashSetTest>.Parse("-h", "a", "-h", "b", "c");
            
            Assert.Equal(3, parsed.hashset.Count());
            Assert.Contains("a", parsed.hashset);
            Assert.Contains("b", parsed.hashset);
            Assert.Contains("c", parsed.hashset);
        }        
     }
}
