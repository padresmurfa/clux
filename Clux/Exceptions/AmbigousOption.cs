namespace Clux
{
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
}
