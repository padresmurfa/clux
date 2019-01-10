using System;

using Clux;

namespace SampleApp
{
    public class SampleArgs
    {
        [Usage("Show the help message")]
        public bool? Help;
        
        [Positional]
        [Required]
        [Usage("The quote to show, as an example.")]
        public string Quote;
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            SampleArgs parsed;
            
            var parser = Parser<SampleArgs>.Create();
            try
            {
                parsed = parser.Parse(args);
            }
            catch (ParserException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                DisplayHelp();
                return;
            }
            
            if (parsed.Help ?? false)
            {
                DisplayHelp();
            }
            else
            {
                Console.WriteLine($"Quote of the day: {parsed.Quote}");
            }
            Console.WriteLine();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("SampleApp Help:");
            Console.WriteLine("--------------------");
            var lines = Parser<SampleArgs>.GetHelpMessage("sampleapp").Split("\n");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
