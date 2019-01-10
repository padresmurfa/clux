using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;

namespace Clux
{
    public class MissingConstantOption : OptionException
    {
        public MissingConstantOption(char option)
            : base(option)
        {
        }

        public MissingConstantOption(string option)
            : base(option)
        {
        }
    }
}
