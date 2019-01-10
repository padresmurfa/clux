namespace Clux
{
    public class UnexpectedPositionalOption : ParserException
    {
        public UnexpectedPositionalOption(int position)
            : base($"Unknown positional option: ${position}")
        {
        }
    }
}
