namespace Clux
{
    public class MissingConstantOption<T> : OptionException<T> where T : new()
    {
        public MissingConstantOption(TargetProperty<T> property)
            : base(property)
        {
            this.UserErrorMessage = $"Missing constant value. '{property.Name}' must have the value '{property.Constant}'";
        }
    }
}
