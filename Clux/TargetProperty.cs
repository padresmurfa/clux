using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;

namespace Clux
{
    public class TargetProperty<T> where T : new()
    {
        public static string ToSnakeCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLowerInvariant();
        }

        public static string ToKebabCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLowerInvariant();
        }

        public string Name { get; private set; }
        public string LongOption { get; private set; }
        public char? ShortOption { get; set; }
        public int Order { get; private set; }
        public int? Position { get; set; }
        public System.Type TargetType { get; private set; }
        public bool Touched { get; set; }
        public bool Passed { get; set; }
        public bool Required { get; private set;}
        public readonly string Usage;
        public string KebabName { get { return ToKebabCase(this.Name); } }
        public object Constant { get; set;}

        public bool IsShortOptionExplicit { get; set; }

        TargetProperty(string name, ConstantAttribute constant, UsageAttribute usage, RequiredAttribute required, AbbreviationAttribute abbreviation, PositionalAttribute positional, System.Type memberType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (usage == null)
            {
                throw new ArgumentNullException(nameof(usage));
            }
            if (memberType == null)
            {
                throw new ArgumentNullException(nameof(memberType));
            }

            this.Name = name;
            
            this.Usage = usage.Usage ?? string.Empty;

            this.Position = positional?.Order;

            if (!this.Position.HasValue)
            {
                this.LongOption = this.KebabName;

                if (abbreviation?.Abbreviation != null)
                {
                    this.IsShortOptionExplicit = true;
                    this.ShortOption = abbreviation.Abbreviation;
                }
                else
                {
                    this.ShortOption = this.LongOption.First();
                }
            }
            
            this.Constant = constant?.Constant;

            this.Order = (new OrderedAttribute[]{ usage, required, abbreviation, positional }).Max(x => x?.Order ?? 0) + 1 /* roughly correct */;

            var isExplicitlyRequired = required != null;

            var isNullableType = (!memberType.IsValueType) || (null != Nullable.GetUnderlyingType(memberType));

            this.Required = isExplicitlyRequired || !isNullableType;

            this.TargetType = memberType;
        }

        public TargetProperty(FieldInfo field)
            : this(field.Name, (ConstantAttribute)field.GetCustomAttribute(typeof(ConstantAttribute)), (UsageAttribute)field.GetCustomAttribute(typeof(UsageAttribute)), (RequiredAttribute)field.GetCustomAttribute(typeof(RequiredAttribute)), (AbbreviationAttribute)field.GetCustomAttribute(typeof(AbbreviationAttribute)), (PositionalAttribute)field.GetCustomAttribute(typeof(PositionalAttribute)), field.FieldType)
        {
        }

        public TargetProperty(PropertyInfo property)
            : this(property.Name, (ConstantAttribute)property.GetCustomAttribute(typeof(ConstantAttribute)), (UsageAttribute)property.GetCustomAttribute(typeof(UsageAttribute)), (RequiredAttribute)property.GetCustomAttribute(typeof(RequiredAttribute)), (AbbreviationAttribute)property.GetCustomAttribute(typeof(AbbreviationAttribute)), (PositionalAttribute)property.GetCustomAttribute(typeof(PositionalAttribute)), property.PropertyType)
        {
        }

        public void SetValue(T instance, object value)
        {
            if (this.Constant != null)
            {
                if (!this.Constant.Equals(value))
                {
                    throw new MissingConstantOption(this.LongOption);
                }
            }
            
            var pi = typeof(T).GetProperty(Name);
            if (pi != null)
            {
                pi.SetValue(instance, value);
                Touched = true;
                return;
            }

            var fi = typeof(T).GetField(Name);
            if (fi != null)
            {
                fi.SetValue(instance, value);
                Touched = true;
                return;
            }
            throw new NotImplementedException("The target is neither a property or a field.  This was unexpected.");
        }

        public object GetValue(T instance)
        {
            var pi = typeof(T).GetProperty(Name);
            if (pi != null)
            {
                return pi.GetValue(instance);
            }

            var fi = typeof(T).GetField(Name);
            if (fi != null)
            {
                return fi.GetValue(instance);
            }
            throw new NotImplementedException("The target is neither a property or a field.  This was unexpected.");
        }

        public static IEnumerable<TargetProperty<T>> TargetPropertiesAndFields
        {
            get
            {
                var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).
                    Where(p => p.CanWrite).
                    Where(p => !p.IsSpecialName).
                    Select(p => new TargetProperty<T>(p));

                var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).
                    Where(f => !f.IsSpecialName).
                    Select(f => new TargetProperty<T>(f));

                var paf = properties.Concat(fields);

                return paf.
                    OrderBy(t => t.Order);
            }
        }
    }
}
