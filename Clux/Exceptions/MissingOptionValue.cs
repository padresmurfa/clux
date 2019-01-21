namespace Clux
{
    public class MissingOptionValue<T> : OptionException<T> where T : new()
    {
        public MissingOptionValue(TargetProperty<T> property)
            : base(property)
        {
            this.UserErrorMessage = $"Missing option value. '{property.Name}' must be provided with a value";
        }
    }
}
