using Assumptions;

namespace Clux
{

    public class Parser<T>
        where T : new()
    {
        public static T Parse(params string[] args)
        {
            try
            {
                return Create().Parse(args);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }

        public static T Parse(out string[] remainder, params string[] args)
        {
            try
            {
                return Create().Parse(out remainder, args);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }

        public static ParserInstance<T> Create()
        {
            try
            {
                return new ParserInstance<T>();
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }

        public static string GetHelpMessage(string command)
        {
            try
            {
                return Create().GetHelpMessage(command);
            }
            catch (System.Exception ex)
            {
                Assume.That(ex).Is.InstanceOf(new []{ typeof(ParserException), typeof(AssumptionFailure) });
                throw;
            }
        }
    }
}
