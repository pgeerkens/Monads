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
using System.Diagnostics.CodeAnalysis;

using Xunit;

namespace PGSolutions.Monads {
    using static Functions;
    using static MaybeLinq;
    using static FormattableString;

    /// <summary>TODO</summary>
    internal static partial class MaybeFunctors {
        /// <summary>TODO</summary>
        public static X<TValue>                     AsMaybe<TValue>(this TValue v) where TValue:class  => v.AsX();
        /// <summary>TODO</summary>
        public static X<Box<TValue>>                Tomaybe<TValue>(this TValue v) where TValue:struct => v.ToBox().AsX();
    }

    /// <summary>Unit tests for <see cref="X{T}"/></summary>
    /// <remarks>
    /// See
    ///     <a href="https://en.wikipedia.org/wiki/Monad_(functional_programming)#fmap_and_join"/>
    /// as well as
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#The_monad_laws_and_their_importance"/>
    /// and
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#cite_ref-1"/> .
    /// </remarks>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeClassMonadTests {
        public MaybeClassMonadTests() { }

        #pragma warning disable IDE1006
        private static Func<string,string> f => s => s + "X";
        private static Func<string,string> g => s => "(" + s + ")";
        private static Func<string,string> h => s => s + s;
#pragma warning restore IDE1006

        private static Func<string,string> Identity => Functions.Identity;

        #region Functor expression of the Monad requirements
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        [Theory]
        [InlineData("4")]
        public static void FunctorLeftIdentity<T>(string v) {
            var lhs = g(v);
            var rhs = Fmap(g)(v.AsMaybe());

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Theory]
        [InlineData("4")]
        public static void FunctorRightIdentity(string v) {
            var m   = v.AsMaybe();
            var lhs = m;
            var rhs = Fmap(Identity)(m);

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Theory]
        [InlineData("4")]
        public static void FunctorAssociativity(string v) {
            var m   = v.AsMaybe();
            var lhs = Fmap(f.Compose(g)) (m);
            var rhs = Fmap(f) (Fmap(g) (m));

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Monad Associativity: (f >=> g) >=> h ≡ f >=> (g >=> h).</summary>
        [Theory]
        [InlineData("4")]
        public static void MonadAssociativity(string v) {
            Func<string, X<string>> fm = u => f(u).AsMaybe();
            Func<string, X<string>> gm = u => g(u).AsMaybe();
            Func<string, X<string>> hm = u => h(u).AsMaybe();

            var lhs = fm.Then(gm).Then(hm) (v);
            var rhs = fm.Then(gm.Then(hm)) (v);

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }
        #endregion

        #region LINQ-syntax expression of the Monad requirements
        /// <summary>Monad Law #1: (return x) >>= f == return f(x).</summary>
        [Theory]
        [InlineData("4")]
        public static void LinqLeftIdentity(string v) {
            var lhs = from x in v.AsMaybe() select f(v);
            var rhs = f(v).AsMaybe();

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Monad Law #2: m >>= return = m.</summary>
        [Theory]
        [InlineData("4")]
        public static void LinqRightIdentity(string v) {
            var m   = v.AsMaybe();
            var lhs = from x in m select x;
            var rhs = m;

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Theory]
        [InlineData("4")]
        public static void LinqAssociativity(string v) {
            var m   = v.AsMaybe();
            var lhs = from y in (from x in m select f(x)) select g(y);
            var rhs = m.SelectMany(x => from y in f(x).AsMaybe() select g(y));

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }
        #endregion

        #region Null-Argument tests
        [Theory]
        [InlineData("4")]
        public static void MaybeXSelect(string v) {
            var m = v.AsMaybe();
            var received = m.Select<string,string>(null);
            Assert.True(received == null);
        }
        [Theory]
        [InlineData("4")]
        public static void MaybeXSelectMany1(string v) {
            var m = v.AsMaybe();
            var received = m.SelectMany<string,string>(null);
            Assert.True(received == null);
        }
        [Theory]
        [InlineData("4")]
        public static void MaybeXSelectMany2(string v) {
            var m = v.AsMaybe();
            var received = m.SelectMany<string,string,string>(null, Second);
            Assert.True(received == null, "1st arg null");
        }
        [Theory]
        [InlineData("4")]
        public static void MaybeXSelectMany3(string v) {
            var m = v.AsMaybe();
            var received  = m.SelectMany<string,string,string>(MaybeFunctors.AsMaybe, null);
            Assert.True(received == null, "2nd arg null");
        }
        #endregion
    }
}
