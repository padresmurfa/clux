using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Miscellaneous
{
    public class RemainderSpecs
    {
        public class Remainder
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
        }
        
        [Fact]
        public void SupportsReturningRemainder()
        {
            Parser<Remainder>.Parse(out var remainder, new string[] { "verb", "remainder", "jar" } );
            Assert.Equal(2, remainder.Length);
            
            Assert.Equal("remainder", remainder[0]);
            Assert.Equal("jar", remainder[1]);
        }

        public class RemainderError
        {
            [Positional]
            [Required]
            [Usage("The command to perform, e.g. run, exec, kill, stop, ...")]
            public string Verb { get; set; }
            
            [Positional]
            public int i;
        }

        [Fact]
        public void SupportsReturningRemainderWhenError()
        {
            string[] remainder = null;
            try
            {
                Parser<RemainderError>.Parse(out remainder, new string[] { "verb", "remainder", "jar" } );
                Assert.True(false);
            }
            catch (Exception)
            {
                Assert.Equal(2, remainder.Length);
                
                Assert.Equal("remainder", remainder[0]);
                Assert.Equal("jar", remainder[1]);
            }
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
        public void SupportsEmbeddedArgs()
        {
            var parsed = Parser<Docker>.Parse(out var remainder, new [] {
                "run", "-i", "--tty", "-h", "localhost", "--attach=stderr", "-n", "smurgle",
                "-p", "localhost:3128", "-p", "localhost:3127",
                "--rm",
                "-u", "root:root",
                "-w", "/var/wobble",
                "centos"
            });

            Assert.Equal("run", parsed.Verb);

            var runCommand = Parser<DockerRun>.Parse(remainder);

            Assert.True(runCommand.Interactive);
            Assert.True(runCommand.Tty);

            Assert.Equal("localhost", runCommand.Hostname);
            Assert.Equal(AttachTo.Stderr, runCommand.Attach);
            Assert.Equal("smurgle", runCommand.Name);
            Assert.Equal(2, runCommand.Publish.Length);
            Assert.True(runCommand.Rm);
            Assert.Equal("root:root", runCommand.User);
            Assert.Equal("/var/wobble", runCommand.Workdir);
        }
    }
}
