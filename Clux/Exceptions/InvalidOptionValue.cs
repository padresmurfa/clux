namespace Clux
{
    public class InvalidOptionValue<T> : OptionException<T> where T : new()
    {
        public InvalidOptionValue(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
