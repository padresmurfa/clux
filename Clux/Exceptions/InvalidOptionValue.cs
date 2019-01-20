using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Clux
{
    public class InvalidOptionValue<T> : OptionException<T> where T : new()
    {
        public string InvalidValue { get; set;}
        
        public InvalidOptionValue(TargetProperty<T> property, string invalidValue)
            : base(property)
        {
            this.InvalidValue = invalidValue;
            
            if (property.IsBoolean)
            {
                this.UserErrorMessage = $"Invalid flag. '{property.Name}' cannot accept the value '{invalidValue}'.  It can only accept true or false values. (i.e. 1/0, true/false, t/f, yes/no or y/n)";
            }
            else if (property.IsDateTime)
            {
                this.UserErrorMessage = $"Invalid date/time. '{property.Name}' cannot accept the value '{invalidValue}'.  It can only accept valid date/time formats according to the current locale (region) settings and a few other standard and convenient formats.  See https://github.com/padresmurfa/clux for more details";
            }
            else if (property.IsInteger)
            {
                var bits = property.IntegerBits;
                var un = (property.IsUnsignedInteger) ? "un" : "";
                
                this.UserErrorMessage = $"Invalid number. '{property.Name}' cannot accept the value '{invalidValue}'.  It can only accept valid {bits}-bit {un}signed integers (optionally specified in base 2, 8, or 16).  See https://github.com/padresmurfa/clux for more details";
            }
            else if (property.IsEnum)
            {
                var options = new List<string>();
                
                var targetType = Nullable.GetUnderlyingType(property.TargetType) ?? property.TargetType;
                var enumValues = System.Enum.GetValues(targetType);
                foreach (var e in enumValues)
                {
                    var enumName = System.Enum.GetName(targetType, e).ToLowerInvariant();
                    var enumValue = Convert.ToInt64(e);
                    
                    options.Add($"'{enumName}' ({enumValue})");
                }
                
                var allowed = options.First();
                if (options.Count() > 1)
                {
                    var leading = options.Take(options.Count() - 1);
                    var trailing = options.Last();
                    
                    allowed = string.Join(", ", leading) + " or " + trailing;
                }
                
                this.UserErrorMessage = $"Invalid value. '{property.Name}' cannot accept the value '{invalidValue}'.  It can only accept values from the following list: {allowed}";
            }
        }
    }
}
