using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.Properties
{
    public class DateTimeSpecs
    {
        Clux.ParserInstance<Clopts> parser = Parser<Clopts>.Create();

        [StructLayout(LayoutKind.Sequential)]
        class Clopts
        {
            public DateTime? KDateTime { get; set;}
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
     }
}
