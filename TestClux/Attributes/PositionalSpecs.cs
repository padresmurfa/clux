using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class PositionalSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        enum KEnumType : int
        {
            red = 0xFF0000,
            green = 0x00FF00,
            blue = 0x0000FF
        }

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
            [Positional]
            public string Pronoun { get; set; }

            [Positional]
            public string Noun { get; set; }

            [Positional]
            public string[] Files { get; set; }

            [Positional]
            public string LastFile { get; set; }

            public bool? Indicator { get; set; }

            public bool? Node;

            [Abbreviation('l')]
            public string Location { get; set; }

            [Abbreviation('L')]
            public string Lies;

            public bool? Oompfh { get; set; }

            public string The;
        }

        [Fact]
        public void SupportsPositionalOptions()
        {
            var clopts = parser.Parse(new[] { "the" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Null(clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Null(clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Null(clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Null(clopts.Files);
            Assert.Equal("in", clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in", "spain" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Single(clopts.Files);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.LastFile);

            clopts = parser.Parse(new[] { "the", "rain", "in", "spain", "lies" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Equal(2, clopts.Files.Length);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.Files[1]);
            Assert.Equal("lies", clopts.LastFile);
        }
        
        class Docker
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
        }

        enum AttachTo
        {
            Stdin,
            Stdout,
            Stderr
        }

        class DockerRun
        {
            [Usage("Run container in background and print container ID")]
            public bool? Detach;

            [Usage("Automatically remove the container when it exits")]
            public bool? Rm;
            
            [Usage("Keep STDIN open even if not attached")]
            public bool? Interactive;

            [Usage("Allocate a pseudo-TTY")]
            public bool? Tty;
            
            [Usage("Add a custom host-to-IP mapping (host:ip)")]
            public string AddHost;

            [Abbreviation('a')]
            [Usage("Attach to STDIN, STDOUT or STDERR")]
            public AttachTo? Attach;

            [Usage("Block IO (relative weight), between 10 and 1000, or 0 to disable (default 0)")]
            public int? BlkioWeight;
            
            [Usage("Container host name")]
            public string Hostname;
            
            [Usage("Assign a name to the container")]
            public string Name;

            [Usage("Publish a container’s port(s) to the host")]
            public string[] Publish;

            [Usage("Username or UID (format: <name|uid>[:<group|gid>])")]
            public string User;

            [Usage("Working directory inside the container")]
            public string Workdir;

            [Positional]
            [Required]
            [Usage("The docker image to run")]
            public string Image;

            [Positional]
            [Usage("The command to run in the docker image")]
            public string Command;

            [Positional]
            [Usage("Any optional arguments to pass to the command in the docker image")]
            public string[] Args { get; set; }
        }
        
        [Fact]
        public void SupportsTrailingPositionalOptions()
        {
            var clopts = parser.Parse(new[] { "the", "rain", "in", "spain", "lies" });

            Assert.Equal("the", clopts.Pronoun);
            Assert.Equal("rain", clopts.Noun);
            Assert.Equal(2, clopts.Files.Length);
            Assert.Equal("in", clopts.Files[0]);
            Assert.Equal("spain", clopts.Files[1]);
            Assert.Equal("lies", clopts.LastFile);
        }

        [Fact]
        public void ProvidesHelpMessagePositionalOptionUsage()
        {
            var lines = Parser<Docker>.GetHelpMessage("docker").Split('\n');

            Assert.True( lines.Any(line => line.Contains("verb") && line.Contains("The command to perform, e.g. run, exec, kill, stop, ...")) );
        }

        class NamedBoolsTest
        {
            [Positional]
            public bool foo;
        }
        
        [Fact]
        public void NamedBoolsShouldBeOptionalByDefault()
        {
            var parsed = Parser<NamedBoolsTest>.Parse(new []{ "true"});
            
            Assert.True(parsed.foo);
        }
     }
}
