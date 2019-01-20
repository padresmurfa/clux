namespace Clux
{
    public class NonBoolMergedShortOption<T> : OptionException where T : new()
    {
        public string UnmergedOption { get; set; }
        
        public NonBoolMergedShortOption(string arg, char option)
            : base(option.ToString())
        {
            this.UnmergedOption = arg;
        }
    }
}
