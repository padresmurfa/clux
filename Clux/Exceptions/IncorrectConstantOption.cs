namespace Clux
{
    public class IncorrectConstantOption<T> : OptionException<T> where T : new()
    {
        public object IncorrectValue { get; set; }
        
        public IncorrectConstantOption(TargetProperty<T> property, object incorrectValue)
            : base(property)
        {
            this.IncorrectValue = incorrectValue;
        }
    }
}
