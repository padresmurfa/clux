using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;

namespace Clux
{
    public class ParserException : System.Exception
    {
        public ParserException(string message)
        {
        }
    }
    
    public class AllUnderlyingParsersFailed : ParserException
    {
        public List<ParserException> Exceptions { get; set;}
        
        public AllUnderlyingParsersFailed(List<ParserException> exceptions)
            : base("All underlying parsesr failed")
        {
            this.Exceptions = exceptions;
        }
    } 

    public class OptionException : ParserException
    {
        protected string option;

        public OptionException(char option)
            : base($"Option exception: ${option}")
        {
            this.option = option.ToString();
        }

        public OptionException(string option)
            : base($"Option exception: ${option}")
        {
            this.option = option;
        }
    }

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

    public class UnexpectedPositionalOption : ParserException
    {
        public UnexpectedPositionalOption(int position)
            : base($"Unknown positional option: ${position}")
        {
        }
    }

    public class MissingOptionValue : OptionException
    {
        public MissingOptionValue(char option)
            : base(option)
        {
        }

        public MissingOptionValue(string option)
            : base(option)
        {
        }
    }

    public class AmbiguousOption : OptionException
    {
        public AmbiguousOption(char option)
            : base(option)
        {
        }

        public AmbiguousOption(string option)
            : base(option)
        {
        }
    }

    public class MissingRequiredOption : OptionException
    {
        public MissingRequiredOption(char option)
            : base(option)
        {
        }

        public MissingRequiredOption(string option)
            : base(option)
        {
        }
    }
    
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

    public class DuplicateOption : OptionException
    {
        public DuplicateOption(char option)
            : base(option)
        {
        }

        public DuplicateOption(string option)
            : base(option)
        {
        }
    }

    public class InvalidOptionValue : OptionException
    {
        public InvalidOptionValue(char option)
            : base(option)
        {
        }

        public InvalidOptionValue(string option)
            : base(option)
        {
        }
    }

    public class PassedOption : OptionException
    {
        public PassedOption(char option)
            : base(option)
        {
        }

        public PassedOption(string option)
            : base(option)
        {
        }
    }
}
