namespace Clux
{
    public class UnknownOption<T> : OptionException<T> where T : new()
    {
        public UnknownOption(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
