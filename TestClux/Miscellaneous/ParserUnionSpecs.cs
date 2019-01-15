using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Miscellaneous
{
    public class ParserUnionSpecs
    {
        public class MeRun
        {
            [Constant("run")]
            [Positional]
            [Required]
            [Usage("The docker command")]
            public string Command;
            
            [Usage("The arguments to the 'run' docker command")]
            [Positional]
            public string[] Args;
        }
        
        public class MeStop
        {
            [Constant("stop")]
            [Positional]
            [Required]
            [Usage("The docker command")]
            public string Command;
            
            [Usage("The arguments to the 'stop' docker command")]
            [Positional]
            public string[] Args;
        }
        
        public class MeExec
        {
            [Constant("exec")]
            [Positional]
            [Required]
            [Usage("The docker command")]
            public string Command;
            
            [Usage("The arguments to the 'exec' docker command")]
            [Positional]
            public string[] Args;
        }
        
        [Fact]
        public void SupportsMultipleOptionsHelp()
        {
            var parser = Parser<MeRun,MeStop,MeExec>.Create();
            
            var help = parser.GetHelpMessage("docker");
            
            Assert.Contains("usage: docker ...", help);
            Assert.Contains("variant: docker run", help);
            Assert.Contains("variant: docker stop", help);
            Assert.Contains("variant: docker exec", help);
        }

        [Fact]
        public void SupportsParsingMultipleOptions()
        {
            var parser = Parser<MeRun,MeStop,MeExec>.Create();
            
           parser.Parse("run")
                .When((MeRun ms) => {
                    Assert.Null(ms.Args);
                }).Else((e) => {
                    Assert.True(false);
                });

            parser.Parse("stop", "bla")
                .When((MeStop ms) => {
                    Assert.Single(ms.Args);
                    Assert.Equal("bla",ms.Args[0]);
                }).Else((e) => {
                    Assert.True(false);
                });
            
           parser.Parse("exec")
                .When((MeExec ms) => {
                    Assert.Null(ms.Args);
                }).Else((e) => {
                    Assert.True(false);
                });
        }
     }
}
