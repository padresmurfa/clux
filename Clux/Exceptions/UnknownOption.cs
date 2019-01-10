using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;

namespace Clux
{
    public class UnknownOption : OptionException
    {
        public UnknownOption(char option)
            : base(option)
        {
        }

        public UnknownOption(string option)
            : base(option)
        {
        }

        public override string ToString()
        {
            return $"UnknownOption, option={this.option}";
        }
    }
}
