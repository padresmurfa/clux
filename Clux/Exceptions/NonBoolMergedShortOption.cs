namespace Clux
{
    public class NonBoolMergedShortOption<T> : OptionException where T : new()
    {
        public string UnmergedOption { get; set; }
        
        public NonBoolMergedShortOption(string arg, char option)
            : base(option.ToString())
        {
            this.UnmergedOption = arg;
            this.UserErrorMessage = $"'-{arg}' contains the non-boolean option '{option}', which requires a value and cannot be merged with other short options";
        }
    }
}
