using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Clux
{
    public interface IParserUnionParserInstance
    {
        void Parse(IParserUnion union, params string[] args);

        void Parse(IParserUnion union, out string[] remainder, params string[] args);

        string GetHelpMessage(string command);
    }
    
    public interface IParserUnion
    {
        IParserUnionParserInstance[] Parsers { get; }
        
        void SetResult(object result);
    }
    
    public class ParserUnion2<T1,T2> : IParserUnion
        where T1 : new()
        where T2 : new()
    {
        public object Result;
        private bool handled;
        
        public IParserUnionParserInstance[] Parsers { get { return new []{ (IParserUnionParserInstance)Parser<T1>.Create(), (IParserUnionParserInstance)Parser<T2>.Create() }; } }
        public void SetResult(object result)
        {
            handled = false;
            this.Result = result;
        }
        
        public ParserUnion2<T1,T2> When<T>(Action<T> call) where T : new()
        {
            if (Result?.GetType()?.IsAssignableFrom(typeof(T)) ?? false)
            {
                handled = true;
                call( (T)Result );
            }
            return this;
        }
        
        public ParserUnion2<T1,T2> Else(Action<object> call)
        {
            if (!handled)
            {
                call(Result);
            }
            return this;
        }
    }
    
    public class Parser<T1,T2>
        : Parser<ParserUnion2<T1,T2>>
        where T1 : new()
        where T2 : new()
    {
    }
    
    public class ParserUnion3<T1,T2,T3> : IParserUnion
        where T1 : new()
        where T2 : new()
        where T3 : new()
    { 
        public object Result;
        private bool handled;
        public IParserUnionParserInstance[] Parsers { get { return new []{ (IParserUnionParserInstance)Parser<T1>.Create(), (IParserUnionParserInstance)Parser<T2>.Create(), (IParserUnionParserInstance)Parser<T3>.Create() }; } }
        public void SetResult(object result)
        {
            handled = false;
            this.Result = result;
        }
        
        public ParserUnion3<T1,T2,T3> When<T>(Action<T> call) where T : new()
        {
            if (Result?.GetType()?.IsAssignableFrom(typeof(T)) ?? false)
            {
                handled = true;
                call( (T)Result );
            }
            return this;
        }
        
        public ParserUnion3<T1,T2, T3> Else(Action<object> call)
        {
            if (!handled)
            {
                call(Result);
            }
            return this;
        }
    }
    
    public class Parser<T1,T2,T3>
        : Parser<ParserUnion3<T1,T2,T3>>
        where T1 : new()
        where T2 : new()
        where T3 : new()
    {
    }
}
