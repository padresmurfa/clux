namespace Clux
{
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
}
