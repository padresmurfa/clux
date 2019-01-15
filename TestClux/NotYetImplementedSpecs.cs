using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux
{
    public class NotYetImplementedSpecs
    {
        [Fact(Skip="Not yet implemented")]
        public void ShouldBeAbleToInputCalendarAndTzInfo()
        {
            throw new NotSupportedException();
        }

        class ParentArgs
        {
            [Positional]
            public string Arg;
            
            public class ChildArgs1
            {
                public string Foo;
                
                public string Blat;
            }
            
            public ChildArgs1 Args1;
            
            public class ChildArgs2
            {
                public string Foo;
                
                public string Blat;
            }
            
            [Positional]
            public ChildArgs2 Args2;
        }
       
        [Fact(Skip="Not yet implemented")]
        public void ShouldBeAbleToInputSubStructs()
        {
            // help message and all...
            throw new NotSupportedException();
        }
        
        [Fact(Skip="Not yet implemented")]
        public void ShouldBeAbleToInputDictionaries()
        {
            // help message and all...
            throw new NotSupportedException();
        }
        
        [Fact(Skip="Not yet implemented")]
        public void ShouldBeAbleToInputJson()
        {
            // help message and all...
            throw new NotSupportedException();
        }
     }
}
