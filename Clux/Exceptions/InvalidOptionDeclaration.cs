namespace Clux
{
    public class InvalidOptionDeclaration : OptionException
    {
        public InvalidOptionDeclaration(char option)
            : base(option)
        {
        }

        public InvalidOptionDeclaration(string option)
            : base(option)
        {
        }
    }
}
