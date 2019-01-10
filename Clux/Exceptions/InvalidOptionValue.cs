namespace Clux
{
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
}
