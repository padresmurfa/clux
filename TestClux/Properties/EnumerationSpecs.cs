using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class EnumerationSpecs
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
            public KEnumType? KEnum;
        }

        [Fact]
        public void SupportsEnumLabelOptions()
        {
            var clopts = parser.Parse(new[] { "--k-enum", "green" });

            Assert.Equal(KEnumType.green, clopts.KEnum);
        }

        [Fact]
        public void SupportsEnumValueOptions()
        {
            var clopts = parser.Parse(new[] { "--k-enum", (0xFF0000).ToString() });

            Assert.Equal((KEnumType?)KEnumType.red, clopts.KEnum);
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
            [Abbreviation('a')]
            [Usage("Attach to STDIN, STDOUT or STDERR")]
            public AttachTo? Attach;
        }

        [Fact]
        public void ProvidesHelpMessageEnumOptions()
        {
            var lines = Parser<DockerRun>.GetHelpMessage("docker-run").Split('\n');

            Assert.True( lines.Any(line => line.Contains("-a,") && line.Contains("(stdin|stdout|stderr)")) );
        }
        
        enum KEnumTypeWithoutValues : int
        {
            red,
            green,
            blue
        }
        
        class CloptsWithoutValues
        {
            public KEnumTypeWithoutValues KEnum;
        }
        
        [Fact]
        public void DoesAcceptEnumOptionByNameWhenValueNotExplicitlyRequested()
        {
            Clux.ParserInstance<CloptsWithoutValues> parser = Parser<CloptsWithoutValues>.Create();

            var clopts = parser.Parse(new[] { "--k-enum", "green" });
            
            Assert.Equal(KEnumTypeWithoutValues.green, clopts.KEnum);
        }
     }
}
