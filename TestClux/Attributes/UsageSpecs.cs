using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Attributes
{
    public class UsageSpecs
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
        public void ProvidesHelpMessageUsageClause()
        {
            var lines = Parser<Docker>.GetHelpMessage("docker").Split('\n');

            var firstLine = lines.Where(x => !string.IsNullOrWhiteSpace(x)).First();

            Assert.Contains( "usage:", firstLine.ToLowerInvariant() );
            Assert.Contains( "docker", firstLine );
        }

        [Fact]
        public void ProvidesHelpMessagePositionalOptionUsage()
        {
            var lines = Parser<Docker>.GetHelpMessage("docker").Split('\n');

            Assert.True( lines.Any(line => line.Contains("verb") && line.Contains("The command to perform, e.g. run, exec, kill, stop, ...")) );
        }
            
        [Fact]
        public void ProvidesHelpMessageShortOptionUsage()
        {
            var help = Parser<DockerRun>.GetHelpMessage("docker-run");
            var lines = help.Split('\n');

            Assert.True( lines.Any(line => line.Contains(" -a,") && line.Contains("Attach to STDIN, STDOUT or STDERR")) );
        }

        [Fact]
        public void ProvidesHelpMessageLongOptionUsage()
        {
            var lines = Parser<DockerRun>.GetHelpMessage("docker-run").Split('\n');

            Assert.True( lines.Any(line => line.Contains(", --attach") && line.Contains("Attach to STDIN, STDOUT or STDERR")) );
        }

        [Fact]
        public void ProvidesHelpMessageEnumOptions()
        {
            var lines = Parser<DockerRun>.GetHelpMessage("docker-run").Split('\n');

            Assert.True( lines.Any(line => line.Contains("-a,") && line.Contains("(stdin|stdout|stderr)")) );
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder1
        {
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg2;
            
            public string Arg3;
            
            [Positional]
            public string Arg4;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder2
        {
            public string Arg3;
            
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg2;
            
            [Positional]
            public string Arg4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder3
        {
            [Positional]
            public string Arg1;
            
            [Positional]
            public string Arg3;
            
            [Positional]
            public string Arg2;
            
            [Positional]
            public string Arg4;
        }
        
        // TODO: consider enforcing a description, and having it derive from this
        //       so the creator does not have to do this manually
        [StructLayout(LayoutKind.Sequential)]
        public class TestOrder4
        {
            public string Arg4;
            
            public string Arg3;
            
            public string Arg2;
            
            public string Arg1;
        }
        
        [Fact]
        public void DescribesArgsInCorrectOrderInHelp()
        {
            var parser1 = Parser<TestOrder1>.Create();
            
            var help = parser1.GetHelpMessage("order");
            Assert.Equal("usage: order [-a <str>] [arg1] [arg2] [arg4]", help.Split("\n").First());
            
            var parser2 = Parser<TestOrder2>.Create();
            
            help = parser2.GetHelpMessage("order");
            Assert.Equal("usage: order [-a <str>] [arg1] [arg2] [arg4]", help.Split("\n").First());
            
            var parser3 = Parser<TestOrder3>.Create();
            
            help = parser3.GetHelpMessage("order");
            Assert.Equal("usage: order [arg1] [arg3] [arg2] [arg4]", help.Split("\n").First());
            
            var parser4 = Parser<TestOrder4>.Create();
            
            help = parser4.GetHelpMessage("order");
            Assert.Equal("usage: order [--arg4 <str>] [--arg3 <str>] [--arg2 <str>] [--arg1 <str>]", help.Split("\n").First());
        }
     }
}
