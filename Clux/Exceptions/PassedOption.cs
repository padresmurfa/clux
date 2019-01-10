namespace Clux
{
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
