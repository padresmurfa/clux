namespace Clux
{
    public abstract class ParserException : System.Exception
    {
        public string[] Input { get; set; }
        public int NextPositional { get; set; }
        public string[] Remainder { get; set; }
        public string UserErrorMessage { get; set; }
        
        public ParserException(string message)
            : base(message)
        {
            this.UserErrorMessage = $"Failed to parse command line arguments: {message}";
        }
    }
}
