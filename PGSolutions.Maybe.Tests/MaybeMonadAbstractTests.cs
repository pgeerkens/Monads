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
    using static FormattableString;
    /// <summary>Unit tests for <see cref="X{T}"/> where T:struct</summary>
    /// <remarks>
    /// See
    ///     <a href="https://en.wikipedia.org/wiki/Monad_(functional_programming)#fmap_and_join"/>
    /// as well as
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#The_monad_laws_and_their_importance"/>
    /// and
    ///     <a href="https://en.wikibooks.org/wiki/Haskell/Category_theory#cite_ref-1"/> .
    /// </remarks>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeMonadAbstractTestsMaybeMonadTests<T> where T:class {
        public MaybeMonadAbstractTestsMaybeMonadTests(Func<T,T> ff, Func<T,T>gg, Func<T,T>hh) {
            f  = ff;
            g  = gg;
            h  = hh;
        }

        #region utility delegates
        #pragma warning disable IDE1006
        private Func<T, T>        f { get; }
        private Func<T, T>        g { get; }
        private Func<T, T>        h { get; }

        private Func<T, X<T>>     fm => u => AsMonad(f(u));
        private Func<T, X<T>>     gm => u => AsMonad(g(u));
        private Func<T, X<T>>     hm => u => AsMonad(h(u));
        #pragma warning restore IDE1006

        protected virtual Func<T, X<T>>                    AsMonad { get; }
        protected virtual Func<Func<T,T>, Func<X<T>,X<T>>> Fmap    { get; }

        private Func<T, T>        Identity => Functions.Identity;
        private Func<T, T, T>     Second   => Functions.Second;
        private Func<T, T>        Null2A   => null;
        private Func<T, X<T>>     Null2B   => null;
        private Func<T, T,T>      Null3    => null;
        #endregion

        #region Functor expression of the Monad requirements
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        public void FunctorLeftIdentityInner(T v) {
            var lhs = AsMonad(f(v));
            var rhs = Fmap(f)(AsMonad(v));

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        public void FunctorRightIdentityInner(T v) {
            var lhs = Fmap(Identity)(v);
            var rhs = v;

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        public void FunctorAssociativityInner(T v) {
            var m   = v.AsMaybe();
            var lhs = Fmap(f.Compose(g)) (m);
            var rhs = Fmap(f) (Fmap(g) (m));

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Monad Associativity: (f >=> g) >=> h ≡ f >=> (g >=> h).</summary>
        public void MonadAssociativityInner(T v) {
            var lhs = fm.Then(gm).Then(hm) (v);
            var rhs = fm.Then(gm.Then(hm)) (v);

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }
        #endregion

        #region LINQ-syntax expression of the Monad requirements
        /// <summary>Monad Law #1: (return x) >>= f == return f(x).</summary>
        public void LinqLeftIdentityInner(T v) {
            var m   = v.AsMaybe();
            var lhs = from x in m select f(v);
            var rhs = f(v).AsMaybe();

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summary>Monad Law #2: m >>= return = m.</summary>
        public void LinqRightIdentityInner(T v) {
            var m   = v.AsMaybe();
            var lhs = from x in m select x;
            var rhs = m;

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }

        /// <summavirtual ry>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        public void LinqAssociativityInner(T v) {
            var m   = v.AsMaybe();
            var lhs = from y in (from x in m select f(x)) select g(y);
            var rhs = m.SelectMany(x => from y in f(x).AsMaybe() select g(y));

            Assert.True(lhs==rhs, Invariant($"lhs: '{lhs}'; rhs: '{rhs}'"));
        }
        #endregion

        #region Null-Argument tests
        public void MaybeXSelectInner(T v) {
            var m = v.AsMaybe();
            var received = m.Select(Null2A);

            Assert.True(received == null);
        }
        public void MaybeXSelectMany1Inner(T v) {
            var m = v.AsMaybe();
            var received = m.SelectMany(Null2B);

            Assert.True(received == null);
        }
        public void MaybeXSelectMany2Inner(T v) {
            var m = v.AsMaybe();
            var received = m.SelectMany(null, Second);

            Assert.True(received == null, "1st arg null");
        }
        public void MaybeXSelectMany3Inner(T v) {
            var m = v.AsMaybe();
            var received = m.SelectMany(AsMonad, Null3);

            Assert.True(received == null, "2nd arg null");
        }
        #endregion
    }
}
