using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class ListSpecs
    {
        class ListTest
        {
            [Positional]
            public List<string> strings;
            
            public List<string> list;
        }
        
        [Fact]
        public void ShouldTreatListLikeArray()
        {
            var parsed = Parser<ListTest>.Parse("a", "b", "c");
            
            Assert.Equal(3, parsed.strings.Count());
            Assert.Equal("a", parsed.strings[0]);
            Assert.Equal("b", parsed.strings[1]);
            Assert.Equal("c", parsed.strings[2]);
            
            parsed = Parser<ListTest>.Parse("-l", "a", "-l", "b", "c");
            
            Assert.Equal(3, parsed.list.Count());
            Assert.Equal("a", parsed.list[0]);
            Assert.Equal("b", parsed.list[1]);
            Assert.Equal("c", parsed.list[2]);
        }        
     }
}
