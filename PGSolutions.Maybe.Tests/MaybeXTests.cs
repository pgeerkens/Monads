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
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;

namespace PGSolutions.Monads.MaybeTests {
    using static Functions;

    /// <summary>TODO</summary>
    /// <remarks>
    /// See
    ///     <a href="https://en.wikipedia.org/wiki/Monad_(functional_programming)#fmap_and_join"/>
    /// as well as
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#The_monad_laws_and_their_importance".
    /// </remarks>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeXTests {
        #region Weasley tests
        readonly static string[]                _strings  =  { "Percy", null, "George", "Ron", "Ginny" };
        readonly static IList<X<string>>        _data     = ( from e in _strings
                                                              select e.AsX()
                                                            ).ToList().AsReadOnly();
        readonly static Func<string, X<string>> _addOne   = x => x + "constant";
        readonly static Func<string, X<string>> _addEight = x => x + "/" + x;

        [Theory]
        [InlineData("",        "Percy//George/Ron/Ginny")]
        [InlineData("Nothing", "Percy/Nothing/George/Ron/Ginny")]
        public static void BasicTest(string defaultValue, string expected) {
            var received = string.Join("/", from e in _data
                                            select e | defaultValue
                                            );
            Contract.Assert(received != null);
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
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Ron/Ginny")]
        [InlineData(null,  "Nothing!")]
        public static void ExcludedMiddleTest(bool? comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.AreNonNullEqual("George") == comparison
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
        [InlineData("Nothing!",      null)]
        public static void WesDyerTest(string expected, string second) {
            var received = ( from x in "Fred".AsX()
                             from y in second.AsX()
                             select x + y).ToNothingString();
            Assert.Equal(expected, received);
        }
        #endregion

        #region Monad tests
        const string                          _v    = "4";
        static readonly X<string>             _m    = "4".AsX();
        static readonly Func<string,string>    f    = s => s + "X";
        static readonly Func<string,string>    g    = s => "(" + s + ")";

        /// <summary>Monad Law #1: (return x).Bind(f) == f(x)</summary>
        [Fact]
        public static void MonadLaw1() {
            var lhs = _v.AsX().SelectMany(_addOne);
            var rhs = _addOne(_v);

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Monad Law #2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2() {
            var lhs = _m.SelectMany<string,string>(i => i);
            var rhs = _m;

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Fact]
        public static void MonadLaw3() {
            var lhs = _m.SelectMany(_addOne).SelectMany(_addEight);
            var rhs = _m.SelectMany(x => _addOne(x).SelectMany(_addEight));

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Fact]
        public static void FunctorLaw1() {
            var lhs = from i in _m select i;
            var rhs = _m;

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Fact]
        public static void FunctorLaw2() {
            var lhs = from s in _m select g(f(s));
            var rhs = from s in ( from s in _m select f(s) ) select g(s);

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        /// <remarks>In expanded form: \x -> return (f x) = \x -> fmap f (return x).</remarks>
        [Fact]
        public static void ReturnLaw() {
            var lhs = g(_v).AsX();
            var rhs = from s in _v.AsX() select g(s);

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #1: ( join . fmap join ) ≡ ( join . join ).</summary>
        [Fact]
        public static void JoinLaw1() {
            var m = ((object)((object)_m).AsX()).AsX();
            var lhs = from x1 in m
                      from x2 in (X<object>)x1
                      from r in (X<string>)x2
                      select r;
            var rhs = from x3 in ( from x1 in m from x2 in (X<object>)x1 select x2 )
                      from r in (X<string>)x3
                      select r as string;

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #2: ( join . fmap return ) ≡ ( join . return = id ).</summary>
        [Fact]
        public static void JoinLaw2() {
            var lhs = from x1 in _m from x2 in x1.AsX() select x2;
            var rhs = from x1 in _m from x2 in x1.AsX() select x2;

            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #3: ( join . fmap (fmap f) ) ≡ ( fmap f . join ).</summary>
        /// <remarks>In expanded form: \x -> join (fmap (fmap f) x) = \x -> fmap f (join x).</remarks>
        [Fact]
        public static void JoinLaw3() {
            var lhs = from x1 in _m.Select(f).Select(g) select x1;
            var rhs = from x2 in ( from x1 in _m select f(x1) ) select g(x2);

             Assert.Equal(lhs, rhs);
        }
        #endregion
    }

    internal class ExternalState {
        private int _state;

        public ExternalState() { _state = -1; }
        public int GetState() { return ++_state; }
    }
}
