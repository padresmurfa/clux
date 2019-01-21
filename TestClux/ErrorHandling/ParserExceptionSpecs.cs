using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Clux;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestClux.ErrorHandling
{
    public class ParserExceptionSpecs
    {
        class ParserExceptionArgs
        {
            public string Option;
            
            [Positional]
            public string Positional1;
            
            [Positional]
            public string Positional2;
        }

        [Fact]
        public void ShouldThrowParserExceptions()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ShouldProvideOriginalArgs()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("--not-an-option", "first", "second", "third");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(4, ex.Input.Length);
                Assert.Equal("--not-an-option", ex.Input[0]);
                Assert.Equal("first", ex.Input[1]);
                Assert.Equal("second", ex.Input[2]);
                Assert.Equal("third", ex.Input[3]);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ShouldIndicateWhatPositionalHasBeenPassed()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(0, ex.NextPositional);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        
            try
            {
                Parser<ParserExceptionArgs>.Parse("first", "--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(1, ex.NextPositional);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
            
            try
            {
                Parser<ParserExceptionArgs>.Parse("first", "second", "--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(2, ex.NextPositional);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ShouldIndicateWhatRemainderOfCommandIsFaulty()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("--not-an-option", ex.Remainder[0]);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        
            try
            {
                Parser<ParserExceptionArgs>.Parse("first", "--not-an-option");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("--not-an-option", ex.Remainder[0]);
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ShouldNotUnwrapMergedShortOptionsWhenDeterminingWhatRemainderOfCommandIsFaulty()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("-not");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(1, ex.Input.Length);
                Assert.Equal("-not", ex.Input[0]);
                
                // ok, this is somewhat weird
                Assert.Equal(1, ex.Remainder.Length);
                Assert.Equal("-not", ex.Remainder[0]);
                
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
        
        [Fact]
        public void ShouldNotUnwrapAnyMergedShortOptionsWhenDeterminingWhatRemainderOfCommandIsFaulty()
        {
            try
            {
                Parser<ParserExceptionArgs>.Parse("-no", "-thi", "-ng");
                Assert.True(false);                
            }
            catch (ParserException ex)
            {
                Assert.Equal(3, ex.Input.Length);
                Assert.Equal("-no", ex.Input[0]);
                Assert.Equal("-thi", ex.Input[1]);
                Assert.Equal("-ng", ex.Input[2]);
                
                // ok, this is somewhat weird
                Assert.Equal(3, ex.Remainder.Length);
                Assert.Equal("-no", ex.Remainder[0]);
                Assert.Equal("-thi", ex.Remainder[1]);
                Assert.Equal("-ng", ex.Remainder[2]);
                
                Assert.False(string.IsNullOrEmpty(ex.UserErrorMessage));
            }
        }
    }
}
