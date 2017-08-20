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

namespace PGSolutions.Monads {
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class EqualityExtensionsTestsMaybe {

        #region Weasley tests
        readonly static string[]            _strings = { "Percy", null, "George", "Ron", "Ginny" };
        readonly static IList<X<string>>    _data    = ( from e in _strings
                                                         select e.AsMonad()
                                                       ).ToList().AsReadOnly();

        [Theory]
        [InlineData("", "Percy//George/Ron/Ginny")]
        [InlineData("Nothing", "Percy/Nothing/George/Ron/Ginny")]
        public static void BasicTest(string defaultValue, string expected) {
            var received = string.Join("/", from e in _data
                                            select e | defaultValue
                                            );
            Assert.True(received != null);
            Assert.Equal(expected, received);
        }

        /// <summary>Verify that == and != are opposites, and are implemented as Equals().</summary>
        [Theory]
        [InlineData(true, "George")]
        [InlineData(false, "Percy/Nothing!/Ron/Ginny")]
        public static void IncludedMiddleTest(bool comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.Equals("George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
            Assert.False(ReferenceEquals(expected, received));

            received     = string.Join("/", from e in _data
                                            where (e == "George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);

            received     = string.Join("/", from e in _data
                                            where (e != "George") != comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(true, "George")]
        [InlineData(false, "Percy/Ron/Ginny")]
        [InlineData(null, "Nothing!")]
        public static void ExcludedMiddleTestEqual(bool? comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.AreNonNullEqual("George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(false, "George")]
        [InlineData(true, "Percy/Ron/Ginny")]
        [InlineData(null, "Nothing!")]
        public static void ExcludedMiddleTestUnequal(bool? comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.AreNonNullUnequal("George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
        }

        [Fact]
        public static void LazyEvaluationTest() {
            var state = new ExternalState();
            var x = (from a in (X<Func<int>>)state.GetState select a) | (()=>0);
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Assert.Equal(0, y);
            Assert.Equal(6, state.GetState());
            Assert.Equal(7, x());
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Theory]
        [InlineData("Fred Weasley", " Weasley")]
        [InlineData("Nothing!", null)]
        public static void WesDyerTest(string expected, string second) {
            var received = ( from x in "Fred".AsMonad()
                             from y in second.AsMonad()
                             select x + y).ToNothingString();
            Assert.Equal(expected, received);
        }
        #endregion
    }
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class EqualityExtensionsTestsNullable {
        #region Other tests
        readonly static int? _maybeGeorge = 2;
        readonly static IList<int?> _data = ( from e in new List<int?>() { 0, null, 2, 3, 4 }
                                              select e
                                            ).ToList().AsReadOnly();

        [Theory]
        [InlineData(99, "0/99/2/3/4")]
        public void BasicTest(int defaultValue, string expected) {
            var received = string.Join("/", from e in _data
                                            select e ?? defaultValue
                                            );
            Assert.True(received != null);
            Assert.Equal(expected, received);
        }

        /// <summary>Verify that == and != are opposites, and are implemented as Equals().</summary>
        [Theory]
        [InlineData(true,  "2")]
        [InlineData(false, "0/Nothing/3/4")]
        public void IncludedMiddleTest(bool comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.Equals(_maybeGeorge) == comparison
                                            select e.ToNothingString()
                                      );
            Assert.True(expected == received);
            Assert.False(ReferenceEquals(expected, received));

            received     = string.Join("/", from e in _data
                                            where (e == _maybeGeorge) == comparison
                                            select e.ToNothingString()
                                      );
            Assert.Equal(expected, received);

            received     = string.Join("/", from int? e in _data
                                            where (e != _maybeGeorge) != comparison
                                            select e.ToNothingString()
                                      );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(true,  "2")]
        [InlineData(false, "0/3/4")]
        [InlineData(null,  "Nothing")]
        public void ExcludedMiddleTestEqual(bool? comparison, string expected) {
            var received = string.Join("/", from int? e in _data
                                            where e.AreNonNullEqual(_maybeGeorge) == comparison
                                            select e.ToNothingString()
                                      );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(false, "2")]
        [InlineData(true,  "0/3/4")]
        [InlineData(null,  "Nothing")]
        public void ExcludedMiddleTestUnequal(bool? comparison, string expected) {
            var received = string.Join("/", from int? e in _data
                                            where e.AreNonNullUnequal(_maybeGeorge) == comparison
                                            select e.ToNothingString()
                                      );
            Assert.Equal(expected, received);
        }

        [Fact]
        public static void LazyEvaluationTest() {
            var state = new ExternalState();
            var x = (from a in (X<Func<int>>)state.GetState select a) | (()=>0);
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Assert.Equal(0, y);
            Assert.Equal(6, state.GetState());
            Assert.Equal(7, x());
        }
        #endregion
    }
}
