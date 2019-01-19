using System.Linq;
using System.Collections.Generic;

namespace Clux
{
    public abstract class OptionException : ParserException
    { 
        public string OptionName { get; set;}
        
        public OptionException(string optionName)
            : base($"Option exception: {optionName}")
        {
            this.OptionName = optionName;
        }
    }
    
    public abstract class OptionException<T> : OptionException
        where T : new()
    {
        public TargetProperty<T> Option { get; set;}
        
        public OptionException(TargetProperty<T> option)
            : base(option.Name)
        {
            this.Option = option;
        }
    }
    
    public abstract class OptionsException<T> : OptionException
        where T : new()
    {
        public string[] Arguments { get; set; }
        public TargetProperty<T>[] Options { get; set; }
        
        public OptionsException(IEnumerable<TargetProperty<T>> options)
            : base(string.Join("+", options.Select(x => x.Name)))
        {
            this.Options = options.ToArray();
        }
        
        public OptionsException(IEnumerable<string> optionNames)
            : base(string.Join("+", optionNames))
        {
            this.Arguments = optionNames.ToArray();
        }
    }
}
