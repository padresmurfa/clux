namespace Clux
{
    public abstract class ParserException : System.Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }
    }
}
