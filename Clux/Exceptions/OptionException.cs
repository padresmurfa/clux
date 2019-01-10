namespace Clux
{
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
}
