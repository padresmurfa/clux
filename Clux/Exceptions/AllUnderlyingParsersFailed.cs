using System.Collections.Generic;

namespace Clux
{
    public class AllUnderlyingParsersFailed : ParserException
    {
        public List<ParserException> Exceptions { get; set;}
        
        public AllUnderlyingParsersFailed(List<ParserException> exceptions)
            : base("All underlying parsesr failed")
        {
            this.Exceptions = exceptions;
        }
    } 
}
