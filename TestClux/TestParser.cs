using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux
{
    public class TestParser
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

            public byte? KByte { get; set; }
            public ushort? KUShort { get; set; }
            public uint? KUInt { get; set; }
            public ulong? KULong { get; set; }

            public sbyte? KSByte { get; set; }
            public short? KShort { get; set; }
            public int? KInt { get; set; }
            public long? KLong { get; set; }

            public decimal? KDecimal { get; set; }
            public float? KFloat { get; set; }
            public double? KDouble { get; set; }

            public KEnumType? KEnum;

            public char? KChar;

            public bool? KTheRainInSpainLiesMainlyOnThePlains;

            public int? KNullableInt { get; set; }

            public int[] KInts { get; set; }

            public DateTime? KDateTime { get; set;}

            public string KString;

            public string[] KMultiple { get; set; }
        }

        [Fact]
        public void SupportsProperties()
        {
            var clopts = parser.Parse(new[] { "--indicator" });

            Assert.True(clopts.Indicator);
        }

        [Fact]
        public void SupportsFields()
        {
            var clopts = parser.Parse(new[] { "--node" });

            Assert.True(clopts.Node);
        }

        [Fact]
        public void SupportsKeyValOptions()
        {
            var clopts = parser.Parse(new[] { "--the=plains" });

            Assert.Equal("plains", clopts.The);
        }

        [Fact]
        public void SupportsLongOptions()
        {
            var clopts = parser.Parse(new[] { "--the", "mainly" });

            Assert.Equal("mainly", clopts.The);
        }

        [Fact]
        public void SupportsShortOptions()
        {
            var clopts = parser.Parse(new[] { "-o" });

            Assert.True(clopts.Oompfh);
        }

        [Fact]
        public void SupportsManualShortOptions()
        {
            var clopts = parser.Parse(new[] { "-l", "location", "-L", "lies" });

            Assert.Equal("location", clopts.Location);
            Assert.Equal("lies", clopts.Lies);
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

        [Fact]
        public void SupportsMergedShortOptions()
        {
            var clopts = parser.Parse(new[] { "-in" });

            Assert.True(clopts.Indicator);
            Assert.True(clopts.Node);
        }

        [Fact]
        public void SupportsShortOptionsWithParams()
        {
            var clopts = parser.Parse(new[] { "-t", "rain" });

            Assert.Equal("rain", clopts.The);
        }

        [Fact]
        public void SupportsLongOptionsWithParams()
        {
            var clopts = parser.Parse(new[] { "--the", "rain" });

            Assert.Equal("rain", clopts.The);
        }

        [Fact]
        public void SupportsBooleanOptions()
        {
            var clopts = parser.Parse(new[] { "--indicator" });

            Assert.True(clopts.Indicator);
        }

        [Fact]
        public void SupportsStringOptions()
        {
            var clopts = parser.Parse(new[] { "--lies", "mainly" });

            Assert.Equal("mainly", clopts.Lies);
        }

        [Fact]
        public void SupportsSbyteIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-s-byte", "33" });

            Assert.Equal((sbyte)33, clopts.KSByte);
        }

        [Fact]
        public void SupportsShortIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-short", "333" });

            Assert.Equal((short)333, clopts.KShort);
        }

        [Fact]
        public void SupportsIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "333" });

            Assert.Equal(333, clopts.KInt);
        }

        [Fact]
        public void SupportsLongIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-long", "333" });

            Assert.Equal(333, clopts.KLong);
        }

        [Fact]
        public void SupportsByteIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-byte", "33" });

            Assert.Equal((byte)33, clopts.KByte);
        }

        [Fact]
        public void SupportsUShortIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-short", "333" });

            Assert.Equal((ushort)333, clopts.KUShort);
        }

        [Fact]
        public void SupportsUIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-int", "333" });

            Assert.Equal((uint)333, clopts.KUInt);
        }

        [Fact]
        public void SupportsULongIntOptions()
        {
            var clopts = parser.Parse(new[] { "--k-u-long", "333" });

            Assert.Equal((ulong)333, clopts.KULong);
        }

        [Fact]
        public void SupportsCharOptions()
        {
            var clopts = parser.Parse(new[] { "--k-char", "a" });

            Assert.Equal('a', clopts.KChar);
        }

        [Fact]
        public void SupportsNullableOptions()
        {
            var clopts = parser.Parse(new[] { "--k-nullable-int", "333" });

            Assert.Equal(333, clopts.KNullableInt);
        }

        [Fact]
        public void SupportsDecimalNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-decimal", "1.303" });

            Assert.Equal(1.303m, clopts.KDecimal);
        }

        [Fact]
        public void SupportsFloatNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-float", "1.303" });

            Assert.Equal(1.303f, clopts.KFloat);
        }

        [Fact]
        public void SupportsDoubleNumberOptions()
        {
            var clopts = parser.Parse(new[] { "--k-double", "1.303" });

            Assert.Equal(1.303, clopts.KDouble);
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

        [Fact]
        public void SupportsArrayOptions()
        {
            var clopts = parser.Parse(new[] { "--k-ints", "0", "1", "2", "3" });

            Assert.Equal(4, clopts.KInts.Length);

            Assert.Equal(0, clopts.KInts[0]);
            Assert.Equal(1, clopts.KInts[1]);
            Assert.Equal(2, clopts.KInts[2]);
            Assert.Equal(3, clopts.KInts[3]);
        }

        [Fact]
        public void SupportsHexadecimalOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0x333" });

            Assert.Equal((int)0x333, clopts.KInt);
        }

        [Fact]
        public void SupportsBinaryOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0b111000" });

            Assert.Equal((int)0b111000, clopts.KInt);
        }

        [Fact]
        public void SupportsOctalOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0o111000" });

            Assert.Equal(S2N.o2n("0o111000", default(int)), clopts.KInt);
        }

        [Fact]
        public void SupportsBaseSpecifierCaseInsensitivelyForIntegerOptions()
        {
            var clopts = parser.Parse(new[] { "--k-int", "0B111" });
            Assert.Equal((int)0b111, clopts.KInt);

            clopts = parser.Parse(new[] { "--k-int", "0O111" });

            Assert.Equal(S2N.o2n("0o111", default(int)), clopts.KInt);

            clopts = parser.Parse(new[] { "--k-int", "0X111" });
            Assert.Equal((int)0x111, clopts.KInt);
        }

        [Fact]
        public void SupportsAllBaseSpecifiersForAllIntegerOptions()
        {
            var binary = parser.Parse(new[] {
                "--k-s-byte", "0b111",
                "--k-short", "0b111",
                "--k-int", "0b111",
                "--k-long", "0b111",
                "--k-byte", "0b111",
                "--k-u-short", "0b111",
                "--k-u-int", "0b111",
                "--k-u-long", "0b111"
            });
            Assert.Equal((sbyte)0b111, binary.KSByte);
            Assert.Equal((short)0b111, binary.KShort);
            Assert.Equal((int)0b111, binary.KInt);
            Assert.Equal((long)0b111, binary.KLong);

            Assert.Equal(((byte)0b111), binary.KByte);
            Assert.Equal((ushort)0b111, binary.KUShort);
            Assert.Equal((uint)0b111, binary.KUInt);
            Assert.Equal((ulong)0b111, binary.KULong);

            var oct = parser.Parse(new[] {
                "--k-s-byte", "0o111",
                "--k-short", "0o111",
                "--k-int", "0o111",
                "--k-long", "0o111",
                "--k-byte", "0o111",
                "--k-u-short", "0o111",
                "--k-u-int", "0o111",
                "--k-u-long", "0o111"
            });
            Assert.Equal(S2N.o2n("0o111", default(sbyte)), oct.KSByte);
            Assert.Equal(S2N.o2n("0o111", default(short)), oct.KShort);
            Assert.Equal(S2N.o2n("0o111", default(int)), oct.KInt);
            Assert.Equal(S2N.o2n("0o111", default(long)), oct.KLong);

            Assert.Equal(S2N.o2n("0o111", default(byte)), oct.KByte);
            Assert.Equal(S2N.o2n("0o111", default(ushort)), oct.KUShort);
            Assert.Equal(S2N.o2n("0o111", default(uint)), oct.KUInt);
            Assert.Equal(S2N.o2n("0o111", default(ulong)), oct.KULong);

            var hex = parser.Parse(new[] {
                "--k-s-byte", "0x11",
                "--k-short", "0x111",
                "--k-int", "0x111",
                "--k-long", "0x111",
                "--k-byte", "0x11",
                "--k-u-short", "0x111",
                "--k-u-int", "0x111",
                "--k-u-long", "0x111"
            });

            Assert.Equal((sbyte)0x11, hex.KSByte);
            Assert.Equal((short)0x111, hex.KShort);
            Assert.Equal((int)0x111, hex.KInt);
            Assert.Equal((long)0x111, hex.KLong);

            Assert.Equal(((byte)0x11), hex.KByte);
            Assert.Equal((ushort)0x111, hex.KUShort);
            Assert.Equal((uint)0x111, hex.KUInt);
            Assert.Equal((ulong)0x111, hex.KULong);
        }

        [Fact]
        public void SupportsKebabCase()
        {
            var clopts = parser.Parse(new[] { "--k-the-rain-in-spain-lies-mainly-on-the-plains" });

            Assert.True(clopts.KTheRainInSpainLiesMainlyOnThePlains);
        }

        [Fact]
        public void SupportsDateTimeOptions()
        {
            var datetime = parser.Parse(new[] { "--k-date-time", "20190103" });
            Assert.Equal((DateTime)new DateTime(2019, 1, 3), datetime.KDateTime);

            datetime = parser.Parse(new[] { "--k-date-time", "20190103100937" });
            Assert.Equal((DateTime)new DateTime(2019, 1, 3, 10, 9, 37), datetime.KDateTime);

            datetime = parser.Parse(new[] { "--k-date-time", "20190103100937888" });
            Assert.Equal((DateTime)new DateTime(2019, 1, 3, 10, 9, 37, 888), datetime.KDateTime);
        }

        public static IEnumerable<object[]> SupportsDateTimeOptionSpecificFormatData
        {
            get
            {
                var now = DateTime.UtcNow;
                
                return new List<object[]>
                {
                    new object[] {
                        "y",
                        new DateTime(2015, 1, 3, 10, 9, 37, 888),
                        new DateTime(2015, 1, 1)
                    },
                    new object[] {
                        "u",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37),
                    },
                    new object[] {
                        "U",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37),
                    },
                    new object[] {
                        "t",
                        new DateTime(2015, 1, 3, 10, 9, 37, 888),
                        new DateTime(now.Year, now.Month, now.Day, 10, 9, 0)
                    },
                    new object[] {
                        "T",
                        new DateTime(2015, 1, 3, 10, 9, 37, 888),
                        new DateTime(now.Year, now.Month, now.Day, 10, 9, 37)
                    },
                    new object[] {
                        "s",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)
                    },
                    new object[] {
                        "r",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)
                    },
                    new object[] {
                        "R",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)
                    },
                    new object[] {
                        "m",
                        new DateTime(2015, 1, 3, 10, 9, 37, 888),
                        new DateTime(now.Year, 1, 3)
                    },
                    new object[] {
                        "M",
                        new DateTime(2015, 1, 3, 10, 9, 37, 888),
                        new DateTime(now.Year, 1, 3)
                    },
                    new object[] {
                        "g",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 0, 0)
                    },
                    new object[] {
                        "G",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)
                    },
                    new object[] {
                        "f",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 0, 0)
                    },
                    new object[] {
                        "F",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)
                    },
                    new object[] {
                        "d",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3)
                    },
                    new object[] {
                        "D",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3)
                    },
                    new object[] {
                        "yyyyMMddHHmmssffffff",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 888)
                    },
                    new object[] {
                        "yyyyMMddHHmmssfffff",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 888)
                    },
                    new object[] {
                        "yyyyMMddHHmmssffff",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 888)
                    },
                    new object[] {
                        "yyyyMMddHHmmssfff",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 888)
                    },
                    new object[] {
                        "yyyyMMddHHmmssff",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 880)
                    },
                    new object[] {
                        "yyyyMMddHHmmssf",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 800)
                    },
                    new object[] {
                        "yyyyMMddHHmmss",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 37, 0)  
                    },
                    new object[] {
                        "yyyyMMddHHmm",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 9, 0, 0)  
                    },
                    new object[] {
                        "yyyyMMddHH",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3, 10, 0, 0, 0)  
                    },
                    new object[] {
                        "yyyyMMdd",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 3)  
                    },
                    new object[] {
                        "yyyyMM",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 1)  
                    },
                    new object[] {
                        "yyyy",
                        new DateTime(2019, 1, 3, 10, 9, 37, 888),
                        new DateTime(2019, 1, 1)  
                    },
                };
            }
        }
        
        [Theory]
        [MemberData(nameof(SupportsDateTimeOptionSpecificFormatData))]
        public void SupportsDateTimeOptionSpecificFormat(string format, DateTime input, DateTime output)
        {
            var now = input.ToLocalTime();
            var kDateTime = now.ToString(format);
            var parsed = parser.Parse(new[] { "--k-date-time", kDateTime });
            Assert.Equal(output, parsed.KDateTime);
        }
        
        [Fact]
        public void DoesNotSupportDuplicateOptionsByDefault()
        {
            try
            {
                parser.Parse(new[] { "--k-string", "a", "--k-string", "b" });
                Assert.False(true);
            }
            catch (DuplicateOption)
            {
            }
        }

        [Fact]
        public void SupportsMultipleInstancesOfSameOption()
        {
            var clopts = parser.Parse(new[] { "--k-multiple", "a", "--k-multiple", "b" });

            Assert.Equal(2, clopts.KMultiple.Length);
            Assert.Equal("a", clopts.KMultiple[0]);
            Assert.Equal("b", clopts.KMultiple[1]);
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
        public void ExplicityShortOptionsTakePrecedenceOverImplicitOnes()
        {
            var clopts = Parser<DockerRun>.Parse(new[] { "-a", "stdin", "centos" });

            Assert.Equal(AttachTo.Stdin, clopts.Attach);
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

        [Fact]
        public void SupportsRequiredFields()
        {
            try
            {
                Parser<Docker>.Parse(out var remainder, new string[0] );
                Assert.False(true);
            }
            catch (MissingRequiredOption)
            {
                var result = Parser<Docker>.Parse(out var remainder, new string[] { "verb" } );
                Assert.Equal("verb", result.Verb);
            }
        }
        
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
            }
            catch (Exception)
            {
                Assert.Equal(2, remainder.Length);
                
                Assert.Equal("remainder", remainder[0]);
                Assert.Equal("jar", remainder[1]);
            }
        }
        
        class OptionalConstStruct
        {
            [Constant(1)]
            public int? ConstantOrDie;
            
            public bool? Detach;
        }
            
        [Fact]
        public void SupportsOptionalConstantOptions()
        {
            // TODO: switch exceptions to contain target-property instead of longOption and such.
            try
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-d" });
            }
            catch (MissingConstantOption)
            {
                Assert.False(true);
            }
            
            try
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-c", "2", "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
                Parser<OptionalConstStruct>.Parse(new []{ "-c", "1", "-d" });
            }
        }
        
        class RequiredConstStruct
        {
            [Constant(1)]
            [Required]
            public int? ConstantOrDie;
            
            public bool? Detach;
        }
        
        [Fact]
        public void SupportsRequiredConstantOptions()
        {
            // TODO: switch exceptions to contain target-property instead of longOption and such.
            try
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
            }
            
            try
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-c", "2", "-d" });
                Assert.False(true);
            }
            catch (MissingConstantOption)
            {
                Parser<RequiredConstStruct>.Parse(new []{ "-c", "1", "-d" });
            }
        }
        
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
        
        public class IgnoreField
        {
            [Ignore]
            public string IgnoreMe;

            [Abbreviation('I')]            
            public string IgnoreMeNot;
        }
        
        [Fact]
        public void ShouldBeAbleToMarkFieldsAsIgnored()
        {
            var parsed = Parser<IgnoreField>.Parse("-I", "ignore-me-not");
            
            try
            {
                parsed = Parser<IgnoreField>.Parse("--ignore-me", "ignore-me");
                Assert.False(true);
            }
            catch (UnknownOption)
            {
            }
        }
        
        public class IgnoreROWO
        {
            public string IgnoreMe { get; private set; }

            public string IgnoreMeToo { private get; set; }

            [Abbreviation('I')]            
            public string IgnoreMeNot;
        }

        [Fact]
        public void ShouldIgnoreNonReadableWritableProperties()
        {
            var parsed = Parser<IgnoreROWO>.Parse("-I", "ignore-me-not");
            
            try
            {
                parsed = Parser<IgnoreROWO>.Parse("--ignore-me", "ignore-me");
                Assert.False(true);
            }
            catch (UnknownOption)
            {
            }
            
            try
            {
                parsed = Parser<IgnoreROWO>.Parse("--ignore-me-too", "ignore-me-too");
                Assert.False(true);
            }
            catch (UnknownOption)
            {
            }
        }                
        
        class OptionalOverrideTests
        {
            [Clux.Optional]            
            public int optionalNotNullable;
            
            [Clux.Optional]            
            public int? optionalNullable;
            
            [Required]
            public int requiredNotNullable;
            
            [Required]
            public int? requiredNullable;
        }
        
        class OptionalOverrideAndRequired
        {
            [Required]
            [Clux.Optional]
            public int conflicting;
        }

        [Fact]
        public void ShouldHaveOptionalAttributeThatIsInverseOfRequired()
        {
            // this should work as a base
            var parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--optional-not-nullable", "1",
                "--optional-nullable", "2",
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(1, parsed.optionalNotNullable);
            Assert.Equal(2, parsed.optionalNullable);
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);

            // confliction option attributes
            try
            {
                Parser<OptionalOverrideAndRequired>.Parse(new []{ "--conflicting", "1" });
                Assert.False(true);
            }
            catch (InvalidOptionDeclaration)
            {
            }
            
            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                //"--optional-not-nullable", "1",
                "--optional-nullable", "2",
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(0, parsed.optionalNotNullable);
            Assert.Equal(2, parsed.optionalNullable);
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);

            parsed = Parser<OptionalOverrideTests>.Parse(new []{
                "--optional-not-nullable", "1",
                //"--optional-nullable", "2",
                "--required-not-nullable", "3",
                "--required-nullable", "4"
            });
            Assert.Equal(1, parsed.optionalNotNullable);
            Assert.Null(parsed.optionalNullable);
            Assert.Equal(3, parsed.requiredNotNullable);
            Assert.Equal(4, parsed.requiredNullable);

            try
            {
                parsed = Parser<OptionalOverrideTests>.Parse(new []{
                    "--optional-not-nullable", "1",
                    "--optional-nullable", "2",
                    //"--required-not-nullable", "3",
                    "--required-nullable", "4"
                });
                Assert.False(true);
            }
            catch (MissingRequiredOption)
            {
            }
            
            try
            {
                parsed = Parser<OptionalOverrideTests>.Parse(new []{
                    "--optional-not-nullable", "1",
                    "--optional-nullable", "2",
                    "--required-not-nullable", "3",
                    //"--required-nullable", "4"
                });
                Assert.False(true);
            }
            catch (MissingRequiredOption)
            {
            }
        }
        
        class NamedBoolsTest
        {
            [Positional]
            public bool foo;
            
            public bool notNullable;
            
            public bool? nullable;
        }
        
        [Fact]
        public void NamedBoolsShouldBeOptionalByDefault()
        {
            var parsed = Parser<NamedBoolsTest>.Parse(new []{ "true"});
            
            Assert.True(parsed.foo);
            Assert.False(parsed.notNullable);
            Assert.Null(parsed.nullable);
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

        struct BoxingUnboxingTest
        {
            [Positional]
            public bool foo;
            
            public bool notNullable;
            
            public bool? nullable;
        }

        [Fact]
        public void ShouldBeImmuneToBoxingUnboxingErrors()
        {
            // see Chris Shain's descussion here: https://stackoverflow.com/questions/9694404/propertyinfo-setvalue-not-working-but-no-errors
            
            var parsed = Parser<BoxingUnboxingTest>.Parse(new []{ "true"});
            
            Assert.True(parsed.foo);
            Assert.False(parsed.notNullable);
            Assert.Null(parsed.nullable);
        }
        
        class ListTest
        {
            [Positional]
            public List<string> strings;
            
            public List<string> list;
            
            public HashSet<string> hashset;
        }
        
        [Fact]
        public void ShouldTreatListAndArrayAndHashSetIndentically()
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
            
            parsed = Parser<ListTest>.Parse("-h", "a", "-h", "b", "c");
            
            Assert.Equal(3, parsed.hashset.Count());
            Assert.Contains("a", parsed.hashset);
            Assert.Contains("b", parsed.hashset);
            Assert.Contains("c", parsed.hashset);
        }        
        
        [Fact(Skip="Not yet implemented")]
        public void ShouldBeAbleToInputCalendarAndTzInfo()
        {
            throw new NotSupportedException();
        }
    }
}
