using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using Assumptions;

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
        
        public TargetProperty<T> GetPositionalOption(int position)
        {
            if (this.ByPosition.Count() <= position)
            {
                throw new ArgumentException($"Internal Error: position({position}) > byPosition.Count", nameof(position));
            }

            //Assume.
            //    That(position, "requested positional number").
            //    Is.Less.Than(this.ByPosition.Count(), "total number of positional options");

            var po = this.ByPosition[position];
            
            Assume.
                That(po.Passed, "requested positional already passed").
                Is.False();

            return po;
        }
    }
}
