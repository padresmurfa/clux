namespace Clux
{
    public class UnexpectedPositionalOption<T> : OptionException where T : new()
    {
        public UnexpectedPositionalOption(int position)
            : base($"#${position}")
        {
        }
    }
}
