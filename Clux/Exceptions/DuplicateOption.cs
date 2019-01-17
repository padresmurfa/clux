namespace Clux
{
    public class DuplicateOption<T> : OptionException<T> where T : new()
    {
        public DuplicateOption(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
