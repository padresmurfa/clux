using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class RequiredSpecs
    {
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
        public void SupportsRequiredFields()
        {
            try
            {
                Parser<Docker>.Parse(out var remainder, new string[0] );
                Assert.False(true);
            }
            catch (MissingRequiredOption<Docker>)
            {
                var result = Parser<Docker>.Parse(out var remainder, new string[] { "verb" } );
                Assert.Equal("verb", result.Verb);
            }
        }
       
        class OptionalOverrideTests
        {
            [Required]
            public int requiredNotNullable;
            
            [Required]
            public int? requiredNullable;
        }

        [Fact]
        public void ShouldHaveOptionalAttributeThatIsInverseOfRequired()
        {
            // this should work as a base
            var parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);
            
            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);

            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);

            try
            {
                parsed = Parser<OptionalOverrideTests>.Parse(new []{
                    //"--required-not-nullable", "3",
                    "--required-nullable", "4"
                });
                Assert.False(true);
            }
            catch (MissingRequiredOption<OptionalOverrideTests>)
            {
            }
            
            try
            {
                parsed = Parser<OptionalOverrideTests>.Parse(new []{
                    "--required-not-nullable", "3",
                    //"--required-nullable", "4"
                });
                Assert.False(true);
            }
            catch (MissingRequiredOption<OptionalOverrideTests>)
            {
            }
        }
     }
}
