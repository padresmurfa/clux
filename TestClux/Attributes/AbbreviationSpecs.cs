using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class AbbreviationSpecs
    {
        Clux.ParserInstance<Args> parser = Parser<Args>.Create();

        class Args
        {
            [Abbreviation('l')]
            public string Location { get; set; }

            [Abbreviation('L')]
            public string Lies;
        }

        [Fact]
        public void SupportsManualShortOptions()
        {
            var clopts = parser.Parse(new[] { "-l", "location", "-L", "lies" });

            Assert.Equal("location", clopts.Location);
            Assert.Equal("lies", clopts.Lies);
        }

        enum AttachTo
        {
            Stdin,
            Stdout,
            Stderr
        }

        class DockerRun
        {
            [Usage("Add a custom host-to-IP mapping (host:ip)")]
            public string AddHost;

            [Abbreviation('a')]
            [Usage("Attach to STDIN, STDOUT or STDERR")]
            public AttachTo? Attach;
        }
   
        [Fact]
        public void ExplicityShortOptionsTakePrecedenceOverImplicitOnes()
        {
            var clopts = Parser<DockerRun>.Parse(new[] { "-a", "stdin" });

            Assert.Equal(AttachTo.Stdin, clopts.Attach);
        }
        
        [Fact]
        public void ProvidesHelpMessageShortOptionUsage()
        {
            var help = Parser<DockerRun>.GetHelpMessage("docker-run");
            var lines = help.Split('\n');

            Assert.True( lines.Any(line => line.Contains(" -a,") && line.Contains("Attach to STDIN, STDOUT or STDERR")) );
        }
     }
}
