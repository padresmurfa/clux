﻿using System.Collections.Generic;
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
        public bool Ignore { get; set; }
        public bool IsShortOptionExplicit { get; set; }
        
        class Attributes
        {
            public UsageAttribute Usage;
            public ConstantAttribute Constant;
            public RequiredAttribute Required;
            public AbbreviationAttribute Abbreviation;
            public PositionalAttribute Positional;
            public IgnoreAttribute Ignore;
            public OptionalAttribute Optional;
            public bool IsTargettable { get; private set; }
            
            public IEnumerable<OrderedAttribute> TargetAttributes
            {
                get
                {
                    return new OrderedAttribute[]{
                        Usage, Constant, Required, Abbreviation, Positional, Ignore
                    }.Where(x => x != null);
                }
            }
            
            public int? Order
            {
                get
                {
                    var attributes = TargetAttributes;
                    
                    if (attributes.Any())
                    {
                        return attributes.Max(x => x?.Order ?? 0) + 1;
                    }
                    
                    return null;
                }
            }            
            
            public Attributes(FieldInfo field)
            {
                Constant = (ConstantAttribute)field.GetCustomAttribute(typeof(ConstantAttribute));
                Usage = (UsageAttribute)field.GetCustomAttribute(typeof(UsageAttribute));
                Required = (RequiredAttribute)field.GetCustomAttribute(typeof(RequiredAttribute));
                Abbreviation = (AbbreviationAttribute)field.GetCustomAttribute(typeof(AbbreviationAttribute));
                Positional = (PositionalAttribute)field.GetCustomAttribute(typeof(PositionalAttribute));
                Ignore = (IgnoreAttribute)field.GetCustomAttribute(typeof(IgnoreAttribute));
                Optional = (OptionalAttribute)field.GetCustomAttribute(typeof(OptionalAttribute));
                
                this.IsTargettable = true;
            }
            
            public Attributes(PropertyInfo property)
            {
                Constant = (ConstantAttribute)property.GetCustomAttribute(typeof(ConstantAttribute));
                Usage = (UsageAttribute)property.GetCustomAttribute(typeof(UsageAttribute));
                Required = (RequiredAttribute)property.GetCustomAttribute(typeof(RequiredAttribute));
                Abbreviation = (AbbreviationAttribute)property.GetCustomAttribute(typeof(AbbreviationAttribute));
                Positional = (PositionalAttribute)property.GetCustomAttribute(typeof(PositionalAttribute));
                Ignore = (IgnoreAttribute)property.GetCustomAttribute(typeof(IgnoreAttribute));
                Optional = (OptionalAttribute)property.GetCustomAttribute(typeof(OptionalAttribute));
                
                var writable = property.GetSetMethod() != null;
                var readable = property.GetGetMethod() != null;
                
                this.IsTargettable = readable && writable;
            }
        }

        public TargetProperty(FieldInfo field)
            : this(field.Name, new Attributes(field), field.FieldType, field.DeclaringType)
        {
        }

        public TargetProperty(PropertyInfo property)
            : this(property.Name, new Attributes(property), property.PropertyType, property.DeclaringType)
        {
        }

        TargetProperty(string name, Attributes attributes, System.Type memberType, System.Type declaringType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (memberType == null)
            {
                throw new ArgumentNullException(nameof(memberType));
            }
            
            this.Ignore = (attributes.Ignore != null) || (!attributes.IsTargettable);
            
            if (!this.Ignore)
            {
                this.Name = name;
    
                this.Usage = attributes.Usage?.Usage ?? string.Empty;
    
                this.Position = attributes.Positional?.Order;
    
                SetLongOption();
    
                SetShortOption(attributes.Abbreviation);
    
                this.Constant = attributes.Constant?.Constant;
    
                SetOrder(memberType, declaringType, attributes);
    
                SetRequired(attributes, memberType);
    
                this.TargetType = memberType;
            }
        }

        private void SetRequired(Attributes attributes, Type memberType)
        {
            var isExplicitlyOptional = attributes.Optional != null;
            var isExplicitlyRequired = attributes.Required != null;
            
            if (isExplicitlyOptional && isExplicitlyRequired)
            {
                throw new InvalidOptionDeclaration(this.LongOption);
            }
            
            if (isExplicitlyRequired)
            {
                this.Required = true;
            }
            else if (isExplicitlyOptional)
            {
                this.Required = false;
            }
            else
            {
                var isNamedBool = memberType.IsValueType && typeof(bool).IsAssignableFrom(memberType) && attributes.Positional == null;
                if (isNamedBool)
                {
                    this.Required = false;
                }
                else
                {
                    var isRefType = !memberType.IsValueType;
                    var isNullableType = null != Nullable.GetUnderlyingType(memberType);
                    if (isRefType || isNullableType)
                    {
                        this.Required = false;
                    }
                    else
                    {
                        this.Required = true;
                    }
                }
            }
        }

        private void SetOrder(Type memberType, Type declaringType, Attributes attributes)
        {
            int hi = attributes.Order ?? 0;
            
            int lo = 0;

            var structLayout = (StructLayoutAttribute)declaringType.GetCustomAttribute(typeof(StructLayoutAttribute));
            if (structLayout?.Value == LayoutKind.Sequential)
            {
                lo = memberType.MetadataToken;
            }
            else if (declaringType.IsValueType && !declaringType.IsEnum)
            {
                // it's a struct, and structs use sequential layout by default
                if (structLayout == null || structLayout.Value == LayoutKind.Auto)
                {
                    lo = memberType.MetadataToken;
                }
                else
                {
                    lo = memberType.FullName.GetHashCode();
                }
            }
            else
            {
                lo = memberType.FullName.GetHashCode();
            }
            
            this.Order = (Int32ToUInt64(hi) << 32) + Int32ToUInt64(lo);
        }
        
        static UInt64 Int32ToUInt64(int val)
        {
            Int64 retval = val;
            
            retval += Int32.MaxValue / 2;
            
            return (UInt64)retval;
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

        public void SetValue(object instance, object value)
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

        public object GetValue(object instance)
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

                var paf = properties.Concat(fields).Where(x => !x.Ignore);

                return paf.
                    OrderBy(t => t.Order);
            }
        }
    }
}
