namespace Clux
{
    public abstract class UnknownOption<T> : OptionException<T> where T : new()
    {
        public UnknownOption(TargetProperty<T> property)
            : base(property)
        {
        }
    }
    
    public class UnknownLongOption<T> : UnknownOption<T> where T : new()
    {
        public UnknownLongOption(TargetProperty<T> property)
            : base(property)
        {
            this.UserErrorMessage = $"Unknown long option: '--{property.LongOption}'";
        }
    }
    
    public class UnknownShortOption<T> : UnknownOption<T> where T : new()
    {
        public UnknownShortOption(TargetProperty<T> property)
            : base(property)
        {
            this.UserErrorMessage = $"Unknown short option: '-{property.ShortOption}'";
        }
    }
}
