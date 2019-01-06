using System;

namespace Clux
{
    public class Parser<T>
        where T : new()
    {
        public static T Parse(params string[] args)
        {
            return Create().Parse(args);
        }

        public static T Parse(out string[] remainder, params string[] args)
        {
            return Create().Parse(out remainder, args);
        }

        public static ParserInstance<T> Create()
        {
            return new ParserInstance<T>();
        }

        public static string GetHelpMessage(string command)
        {
            return Create().GetHelpMessage(command);
        }
    }
}
