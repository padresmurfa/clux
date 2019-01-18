using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;

namespace Clux
{
    public class ParserInstancePositionalOptions<T>
        where T : new()
    {
        List<TargetProperty<T>> ByPosition;
        
        public ParserInstancePositionalOptions(List<TargetProperty<T>> all)
        {
            this.ByPosition = all.
                Where(tp => tp.Position.HasValue).
                OrderBy(tp => tp.Position).
                ToList();
                
            for (var i = 0; i< this.ByPosition.Count; i++)
            {
                this.ByPosition[i].Position = i;
            }
        }
        
        public int Remaining(int? position)
        {
            return this.ByPosition.Count(p => p.Position.HasValue && position.HasValue && p.Position > position.Value);
        }
        
        public TargetProperty<T> GetPositionalOption(int position, IEnumerable<TargetProperty<T>> all)
        {
            if (this.ByPosition.Count() <= position)
            {
                throw new UnexpectedPositionalOption(position);
            }

            var po = this.ByPosition[position];
            
            if (po.Passed)
            {
                throw new InvalidOperationException("Not sure how this happened...");
            }
            
            return po;
        }
    }
}
