namespace Clux
{
    public class InvalidOptionValue<T> : OptionException<T> where T : new()
    {
        public string Value { get; set;}
        
        public InvalidOptionValue(TargetProperty<T> property, string value)
            : base(property)
        {
            this.Value = value;
        }
    }
}
