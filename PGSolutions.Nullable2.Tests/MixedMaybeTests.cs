#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Monad Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2016 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Xunit;

namespace PGSolutions.Monads.Nullable2Tests {
    using static FormattableString;
    using static String;

    [ExcludeFromCodeCoverage]
    public class MixedBasicTests {
        const           string          _maybeGeorge = "George";
        readonly static string[]        _strings     = { "Percy", null, "George","Ron", "Ginny" };
        readonly static IList<string>   _data        = _strings.ToList().AsReadOnly();

        readonly Func<string,Func<string,bool>,IEnumerable<string>> _isGeorge = (s,test) => 
                from e in _data where test(e) select e ?? s;

        /// <summary>Test basic functionaity of bind operation.</summary>
        [Theory]
        [InlineData("",       "Percy//George/Ron/Ginny")]
        [InlineData("Nothing","Percy/Nothing/George/Ron/Ginny")]
        public void BasicTest(string defaultValue, string expected) {
            Func<string,bool> predicate;
            string received;

            predicate = s => true;
            received  = Join("/", _isGeorge(defaultValue, predicate) );

            Assert.True(received != null);
            Assert.True(expected?.Equals(received),
                Invariant($"Value: Expected: '{expected}'; Received: '{received}'"));
        }

        /// <summary>Verify that == and != are opposites, and the former implemented as Equals().</summary>
        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Nothing/Ron/Ginny")]
        public void IncludedMiddleTest(bool comparison, string expected) {
            Func<string,bool> predicate;
            string received;

            predicate = s => (s == _maybeGeorge) == comparison;
            received  = Join("/", _isGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);

                predicate = s => (s != _maybeGeorge) != comparison;
                received  = Join("/", _isGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Ron/Ginny")]
        [InlineData(null,  "Nothing")]
        public void ExcludedMiddleTest(bool? c, string expected) {
            Func<string,bool> predicate;
            Nullable2<bool>   comparison = c??new Nullable2<bool>(); // c.HasValue ? c.Value : new Nullable2<bool>();

            predicate     = s => s.AreNonNullEqual(_maybeGeorge).Equals(comparison);
            var received  = Join("/", _isGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);
        }

        [Fact]
        public static void LazyEvaluationTest() {
            var state = new ExternalState();
            var x = (from a in (Nullable2<Func<int>>)state.GetState select a) | (()=>0);
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Assert.Equal(0, y);
            Assert.Equal(6, state.GetState());
            Assert.Equal(7, x());
        }
    }
}
