namespace Clux
{
    public class MissingRequiredOption<T> : OptionException<T> where T : new()
    {
        public MissingRequiredOption(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
