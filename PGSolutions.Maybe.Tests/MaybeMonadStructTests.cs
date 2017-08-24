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
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeMonadStructTests : MaybeMonadAbstractTestsMaybeMonadTests<Box<int>> {
        public MaybeMonadStructTests() 
            : base(s => s.Value + 4
                 , s => (s.Value + 1) * 3
                 , s => s.Value + s.Value) {
        }

        protected override Func<Box<int>, X<Box<int>>>                                  AsMonad => MaybeLinq.AsMaybe;
        protected override Func<Func<Box<int>,Box<int>>, Func<X<Box<int>>,X<Box<int>>>> Fmap    => MaybeLinq.Fmap;

        private const int StartData = 4;

        #region Functor expression of the Monad requirements
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        [Theory]
        [InlineData(4)]
        public void FunctorLeftIdentity(int v)      => base.FunctorLeftIdentityInner(v.ToBox());

        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Theory]
        [InlineData(4)]
        public void FunctorRightIdentity(int v)     => base.FunctorRightIdentityInner(v.ToBox());

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Theory]
        [InlineData(4)]
        public void FunctorAssociativity(int v)     => base.FunctorAssociativityInner(v.ToBox());

        /// <summary>Monad Associativity: (f >=> g) >=> h ≡ f >=> (g >=> h).</summary>
        [Theory]
        [InlineData(4)]
        public void MonadAssociativity(int v)       => base.MonadAssociativityInner(v.ToBox());
        #endregion

        #region LINQ-syntax expression of the Monad requirements
        /// <summary>Monad Law #1: (return x) >>= f == return f(x).</summary>
        [Theory]
        [InlineData(4)]
        public void LinqLeftIdentity(int v)         => base.LinqLeftIdentityInner(v.ToBox());

        /// <summary>Monad Law #2: m >>= return = m.</summary>
        [Theory]
        [InlineData(4)]
        public void LinqRightIdentity(int v)        => base.LinqRightIdentityInner(v.ToBox());

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Theory]
        [InlineData(4)]
        public void LinqAssociativity(int v)        => LinqAssociativityInner(v.ToBox());
        #endregion

        #region Null-Argument tests
        [Theory]
        [InlineData(4)]
        public void MaybeXSelect(int v)             => base.MaybeXSelectInner(v.ToBox());
        [Theory]
        [InlineData(4)]
        public void MaybeXSelectMany1(int v)        => base.MaybeXSelectMany1Inner(v.ToBox());
        [Theory]
        [InlineData(4)]
        public void MaybeXSelectMany2(int v)        => base.MaybeXSelectMany2Inner(v.ToBox());
        [Theory]
        [InlineData(4)]
        public void MaybeXSelectMany3(int v)        => base.MaybeXSelectMany3Inner(v.ToBox());
        #endregion
    }
}
