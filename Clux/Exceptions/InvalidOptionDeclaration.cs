namespace Clux
{
    public class InvalidOptionDeclaration<T> : OptionException<T> where T : new()
    {
        public InvalidOptionDeclaration(TargetProperty<T> property)
            : base(property)
        {
        }
    }
}
