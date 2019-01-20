namespace Clux
{
    public class UnknownMergedShortOption<T> : OptionException where T : new()
    {
        public string UnmergedOption { get; set; }
        
        public UnknownMergedShortOption(string arg, char option)
            : base(option.ToString())
        {
            this.UnmergedOption = arg;
        }
    }
}
