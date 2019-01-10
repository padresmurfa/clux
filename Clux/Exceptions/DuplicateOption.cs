namespace Clux
{
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
}
