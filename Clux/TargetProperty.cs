using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Clux
{
    public class TargetProperty<T> where T : new()
    {
        static string KebabChar(char c, int index)
        {
            if (index > 0)
            {
                var isUpper = char.IsUpper(c);
                if (isUpper)
                {
                    c = char.ToLowerInvariant(c);
                    return $"-{c}";
                }
            }
            return char.ToLowerInvariant(c).ToString();
        }
        
        public static string ToKebabCase(string str)
        {
            var kebab = str.Select(KebabChar);
            return string.Concat(kebab);
        }

        public string Name { get; private set; }
        public string LongOption { get; private set; }
        public char? ShortOption { get; set; }
        public UInt64 Order { get; private set; }
        public int? Position { get; set; }
        public System.Type TargetType { get; private set; }
        public bool Touched { get; set; }
        public bool Passed { get; set; }
        public bool Required { get; private set;}
        public readonly string Usage;
        public string KebabName { get { return ToKebabCase(this.Name); } }
        public object Constant { get; set;}

        public bool IsShortOptionExplicit { get; set; }

        public TargetProperty(FieldInfo field)
            : this(field.Name, (ConstantAttribute)field.GetCustomAttribute(typeof(ConstantAttribute)), (UsageAttribute)field.GetCustomAttribute(typeof(UsageAttribute)), (RequiredAttribute)field.GetCustomAttribute(typeof(RequiredAttribute)), (AbbreviationAttribute)field.GetCustomAttribute(typeof(AbbreviationAttribute)), (PositionalAttribute)field.GetCustomAttribute(typeof(PositionalAttribute)), field.FieldType, field.DeclaringType)
        {
        }

        public TargetProperty(PropertyInfo property)
            : this(property.Name, (ConstantAttribute)property.GetCustomAttribute(typeof(ConstantAttribute)), (UsageAttribute)property.GetCustomAttribute(typeof(UsageAttribute)), (RequiredAttribute)property.GetCustomAttribute(typeof(RequiredAttribute)), (AbbreviationAttribute)property.GetCustomAttribute(typeof(AbbreviationAttribute)), (PositionalAttribute)property.GetCustomAttribute(typeof(PositionalAttribute)), property.PropertyType, property.DeclaringType)
        {
        }

        TargetProperty(string name, ConstantAttribute constant, UsageAttribute usage, RequiredAttribute required, AbbreviationAttribute abbreviation, PositionalAttribute positional, System.Type memberType, System.Type declaringType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (memberType == null)
            {
                throw new ArgumentNullException(nameof(memberType));
            }

            this.Name = name;

            this.Usage = usage?.Usage ?? string.Empty;

            this.Position = positional?.Order;

            SetLongOption();

            SetShortOption(abbreviation);

            this.Constant = constant?.Constant;

            SetOrder(memberType, declaringType, usage, required, abbreviation, positional);

            SetRequired(required, memberType);

            this.TargetType = memberType;
        }

        private void SetRequired(RequiredAttribute required, Type memberType)
        {
            var isExplicitlyRequired = required != null;
            if (isExplicitlyRequired)
            {
                this.Required = true;
            }
            else
            {
                var isNullableType = (!memberType.IsValueType) || (null != Nullable.GetUnderlyingType(memberType));
                if (!isNullableType)
                {
                    this.Required = true;
                }
                else
                {
                    this.Required = false;
                }
            }
        }

        private void SetOrder(Type memberType, Type declaringType, params OrderedAttribute[] attributes)
        {
            if (attributes.Any(x => x != null))
            {
                var order = unchecked((UInt32)attributes.Max(x => x?.Order ?? 0) + 1);
                this.Order = ((UInt64)order) << 32;
            }

            var structLayout = (StructLayoutAttribute)declaringType.GetCustomAttribute(typeof(StructLayoutAttribute));
            if (structLayout?.Value == LayoutKind.Sequential)
            {
                this.Order += unchecked((UInt32)memberType.MetadataToken);
            }
            else if (declaringType.IsValueType && !declaringType.IsEnum)
            {
                // it's a struct, and structs use sequential layout by default
                if (structLayout == null || structLayout.Value == LayoutKind.Auto)
                {
                    this.Order += unchecked((UInt32)memberType.MetadataToken);
                }
            }
        }

        private void SetLongOption()
        {
            if (!this.Position.HasValue)
            {
                this.LongOption = this.KebabName;
            }
        }

        private void SetShortOption(AbbreviationAttribute abbreviation)
        {
            if (!this.Position.HasValue)
            {
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
            }
            else
            {
                var fi = typeof(T).GetField(Name);
                if (fi != null)
                {
                    fi.SetValue(instance, value);
                    Touched = true;
                }
                else
                {
                    throw new NotImplementedException("The target is neither a property or a field.  This was unexpected.");
                }
            }
        }

        public object GetValue(T instance)
        {
            var pi = typeof(T).GetProperty(Name);
            if (pi != null)
            {
                return pi.GetValue(instance);
            }
            else
            {
                var fi = typeof(T).GetField(Name);
                if (fi != null)
                {
                    return fi.GetValue(instance);
                }
                else
                {
                    throw new NotImplementedException("The target is neither a property or a field.  This was unexpected.");
                }
            }
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
