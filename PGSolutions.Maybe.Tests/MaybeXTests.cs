﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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
    using Maybe_T = X<object>;

    /// <summary>TODO</summary>
    /// <remarks>
    /// See
    ///     <a href="https://en.wikipedia.org/wiki/Monad_(functional_programming)#fmap_and_join"/>
    /// as well as
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#The_monad_laws_and_their_importance".
    /// </remarks>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeXTests {
        const string                            _v  = "4";
        static readonly X<string>               _m  = _v;
        static readonly Func<string,string>     _f  = s => s + "X";
        static readonly Func<string,string>     _g  = s => "(" + s + ")";
        static readonly Func<string,X<string>>  _fm = s => _f(s);
        static readonly Func<string,X<string>>  _gm = s => _g(s);

        #region Functor Laws
        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Fact]
        public static void FunctorLaw1() {
            var lhs = _m;
            var rhs = from i in _m select i;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Fact]
        public static void FunctorLaw2() {
            var lhs = from s in _m select _g(_f(s));
            var rhs = from s in ( from s in _m select _f(s) ) select _g(s);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }
        #endregion
        #region Join Laws
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        /// <remarks>In expanded form: \x -> return (f x) = \x -> fmap f (return x).</remarks>
        [Fact]
        public static void ReturnLaw() {
            var lhs = _g(_v).ToMonad();
            var rhs = from s in _v.ToMonad() select _g(s);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #1: ( join . fmap join ) ≡ ( join . join ).</summary>
        [Fact]
        public static void JoinLaw1() {
            var m = ((object)((object)_m).ToMonad()).ToMonad();
            var lhs = from x1 in m
                      from x2 in (Maybe_T)x1
                      from r  in (X<string>)x2
                      select r;
            var rhs = from x3 in ( from x1 in m from x2 in (X<object>)x1 select x2 )
                      from r in (X<string>)x3
                      select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #2: ( join . fmap return ) ≡ ( join . return = id ).</summary>
        [Fact]
        public static void JoinLaw2() {
            var lhs = from m in MaybeLinq.Select(_v.ToMonad(), _f)
                      from r in MaybeLinq.SelectMany<string,string>(m, i => i.ToMonad())
                      select r;
            var rhs = MaybeLinq.SelectMany(_v.ToMonad(),_fm);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
            //Assert.Equal(_v.ToMonad(),rhs);
        }

        /// <summary>Join Law #3: ( join . fmap (fmap f) ) ≡ ( fmap f . join ).</summary>
        /// <remarks>In expanded form: \x -> join (fmap (fmap f) x) = \x -> fmap f (join x).</remarks>
        [Fact]
        public static void JoinLaw3() {
            var lhs = from x1 in _m.Select(_f).Select(_g) select x1;
            var rhs = from x2 in ( from x1 in _m select _f(x1) ) select _g(x2);

            Assert.True(rhs.HasValue);
             Assert.Equal(lhs, rhs);
        }
        #endregion
        #region Monad Laws
        /// <summary>Monad Law #1: (return x) >>= f == f(x).</summary>
        [Fact]
        public static void MonadLaw1() {
            var lhs = _fm(_v);
            var rhs = _v.ToMonad().SelectMany(_fm);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Monad Law #2: m >>= return = m.</summary>
        [Fact]
        public static void MonadLaw2() {
            var lhs = _m;
            var rhs = _m.SelectMany(Extensions.ToMonad);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Fact]
        public static void MonadLaw3A() {
            var lhs = from y in (from x in _m from r in _fm(x) select r ) from r in _gm(y) select r;
            var rhs = from x in _m from y in _fm(x) from r in _gm(y) select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Fact]
        public static void MonadLaw3B() {
            var lhs = _m.SelectMany(_fm).SelectMany(_gm);
            var rhs = _m.SelectMany(x => _fm(x).SelectMany(_gm));

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }
        #endregion

        #region Weasley tests
        readonly static string[]            _strings = { "Percy", null, "George", "Ron", "Ginny" };
        readonly static IList<X<string>>    _data    = ( from e in _strings
                                                         select e.ToMonad()
                                                       ).ToList().AsReadOnly();

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
            var received = ( from x in "Fred".ToMonad()
                             from y in second.ToMonad()
                             select x + y).ToNothingString();
            Assert.Equal(expected, received);
        }
        #endregion
    }

    internal static partial class Extensions {
        public static X<TValue> ToMonad<TValue>(this TValue value) where TValue:class => value;
    }

    internal class ExternalState {
        private int _state;

        public ExternalState() { _state = -1; }
        public int GetState() { return ++_state; }
    }
}
