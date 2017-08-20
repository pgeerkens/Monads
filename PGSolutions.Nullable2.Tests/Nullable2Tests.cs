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
using System.Diagnostics.CodeAnalysis;

using Xunit;

namespace PGSolutions.Monads.Nullable2Tests {
    using Maybe_T = Nullable2<object>;
    using static Extensions;

    /// <summary>Unit tests for <see cref="X{T}"/></summary>
    /// <remarks>
    /// See
    ///     <a href="https://en.wikipedia.org/wiki/Monad_(functional_programming)#fmap_and_join"/>
    /// as well as
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#The_monad_laws_and_their_importance"/>
    /// and
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#cite_ref-1"/> .
    /// </remarks>
    [ExcludeFromCodeCoverage] //[CLSCompliant(false)]
    public class MonadTests {
        public MonadTests() { }

        const string                                    _v  = "4";
        static readonly Nullable2<string>               _m  = _v;
        static readonly Func<string,string>             _f  = x => x + "X";
        static readonly Func<string,string>             _g  = x => "(" + x + ")";
        static readonly Func<string,Nullable2<string>>  _fm = x => _f(x);
        static readonly Func<string,Nullable2<string>>  _gm = x => _g(x);

        #region Functor Laws
        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Fact]
        public static void FunctorLaw1() {
            var lhs = _m;
            var rhs = from x in _m select x;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Fact]
        public static void FunctorLaw2() {
            var lhs = from x in _m select _g(_f(x));
            var rhs = from x in ( from x in _m select _f(x) ) select _g(x);

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
            var lhs = from x3 in ( from x1 in m from x2 in x1.ToMonad() select x2 )
                      from r in (Maybe_T)x3
                      select r;
            var rhs = from x1 in m
                      from x2 in x1.ToMonad()
                      from r  in (Maybe_T)x2
                      select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #2: join . fmap return  ≡  join . return  =  id.</summary>
        [Fact]
        public static void JoinLaw2() {
            var lhs = _v.ToMonad().SelectMany(Extensions.ToMonad);
            var rhs = from m in _v.ToMonad()//.Select(i=>i)
                 //     from r in m.SelectMany(Extensions.ToMonad)
                      select m;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs, rhs);
            Assert.True(rhs.SelectMany<string, bool>(r => r==_v) | false);
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
            var lhs = from y in ( from x in _m from r1 in _fm(x) select r1 ) from r in _gm(y) select r;
            var rhs = from x in _m from r in ( from y in _fm(x) from r1 in _gm(y) select r1 ) select r;

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

        #region Null-Argument tests
        [Fact]
        public void MaybeXSelect() {
            var received = _m.Select<string,string>(null);
            Assert.False(received.HasValue);
        }
        [Fact]
        public static void MaybeXSelectMany1() {
            var received = _m.SelectMany<string,string>(null);
            Assert.False(received.HasValue);
        }
        [Fact]
        public static void MaybeXSelectMany2() {
            var received = _m.SelectMany<string,string,string>(null, Second);
            Assert.False(received.HasValue, "1st arg null");
        }
        [Fact]
        public static void MaybeXSelectMany3() {
            var received  = _m.SelectMany<string,string,string>(u=>u, null);
            Assert.False(received.HasValue, "2nd arg null");
        }
        #endregion
    }

    public class NullTests {
        //======================================================================
        static readonly Nullable2<string> _maybe  = "Maybe?";
        [Fact]
        public static void Select1() {
            var received = _maybe.Select<string,int>(null);
            Assert.False(received.HasValue);
        }
        [Fact]
        public static void SelectMany1() {
            var received = _maybe.SelectMany<string,int>(null);
            Assert.False(received.HasValue);
        }
        [Fact]
        public static void SelectMany2() {
            var received = _maybe.SelectMany<string,int,int>(null, Second);
            Assert.False(received.HasValue, "1st arg null");
        }
        [Fact]
        public static void SelectMany3() {
            var received = _maybe.SelectMany<string,int,int>(u=>2, null);
            Assert.False(received.HasValue, "2nd arg null");
        }
    }

    internal static partial class Extensions {
        public static Nullable2<TValue> ToMonad<TValue>(this TValue value) => value;

        public static T2 Second<T1,T2>(T1 u, T2 v) => v;
    }
}
