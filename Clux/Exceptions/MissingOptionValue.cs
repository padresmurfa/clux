namespace Clux
{
    public class MissingOptionValue<T> : OptionException<T> where T : new()
    {
        public MissingOptionValue(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
