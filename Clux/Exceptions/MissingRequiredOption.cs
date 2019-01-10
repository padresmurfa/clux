namespace Clux
{
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
}
