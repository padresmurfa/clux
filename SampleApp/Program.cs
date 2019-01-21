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
        static int Main(string[] args)
        {
            SampleArgs parsed;
            
            var parser = Parser<SampleArgs>.Create();
            try
            {
                parsed = parser.Parse(args);
            }
            catch (ParserException ex)
            {
                Console.WriteLine($"ERROR: {ex.UserErrorMessage}");
                DisplayHelp();
                return -1;
            }
            
            if (parsed.Help ?? false)
            {
                Console.WriteLine("SampleApp Help:");
                DisplayHelp();
            }
            else
            {
                Console.WriteLine($"Quote of the day: {parsed.Quote}");
            }
            Console.WriteLine();
            return 0;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("----------------------------------------");
            var lines = Parser<SampleArgs>.GetHelpMessage("sampleapp").Split("\n");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
