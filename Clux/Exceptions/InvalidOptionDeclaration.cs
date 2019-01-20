namespace Clux
{
    public class InvalidOptionDeclaration<T> : OptionException<T> where T : new()
    {
        public InvalidOptionDeclaration(TargetProperty<T> property)
            : base(property)
        {
            this.UserErrorMessage = $"Conflicting option declaration. '{property.Name}' was explicitly declared to be both Optional and Required";
        }
    }
}
