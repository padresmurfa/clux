namespace Clux
{
    public class PassedOption<T> : OptionException<T> where T : new()
    {
        public PassedOption(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
