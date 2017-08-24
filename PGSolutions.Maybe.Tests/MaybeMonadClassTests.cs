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
    public class MaybeMonadClassTests : MaybeMonadAbstractTestsMaybeMonadTests<string> {
        public MaybeMonadClassTests() 
            : base(s => s + "X"
                 , s => "(" + s + ")"
                 , s => s + s) {
        }
        protected override Func<string, X<string>>                              AsMonad => MaybeLinq.AsMaybe;
        protected override Func<Func<string,string>, Func<X<string>,X<string>>> Fmap    => MaybeLinq.Fmap;

        private const string StartData = "4";

        #region Functor expression of the Monad requirements
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        [Theory]
        [InlineData(StartData)]
        public void FunctorLeftIdentity(string v)      => base.FunctorLeftIdentityInner(v);

        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Theory]
        [InlineData(StartData)]
        public void FunctorRightIdentity(string v)     => base.FunctorRightIdentityInner(v);

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Theory]
        [InlineData(StartData)]
        public void FunctorAssociativity(string v)     => base.FunctorAssociativityInner(v);

        /// <summary>Monad Associativity: (f >=> g) >=> h ≡ f >=> (g >=> h).</summary>
        [Theory]
        [InlineData(StartData)]
        public void MonadAssociativity(string v)       => base.MonadAssociativityInner(v);
        #endregion

        #region LINQ-syntax expression of the Monad requirements
        /// <summary>Monad Law #1: (return x) >>= f == return f(x).</summary>
        [Theory]
        [InlineData(StartData)]
        public void LinqLeftIdentity(string v)         => base.LinqLeftIdentityInner(v);

        /// <summary>Monad Law #2: m >>= return = m.</summary>
        [Theory]
        [InlineData(StartData)]
        public void LinqRightIdentity(string v)        => base.LinqRightIdentityInner(v);

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Theory]
        [InlineData(StartData)]
        public void LinqAssociativity(string v)        => base.LinqAssociativityInner(v);
        #endregion

        #region Null-Argument tests
        [Theory]
        [InlineData(StartData)]
        public void MaybeXSelect(string v)             => base.MaybeXSelectInner(v);
        [Theory]
        [InlineData(StartData)]
        public void MaybeXSelectMany1(string v)        => base.MaybeXSelectMany1Inner(v);
        [Theory]
        [InlineData(StartData)]
        public void MaybeXSelectMany2(string v)        => base.MaybeXSelectMany2Inner(v);
        [Theory]
        [InlineData(StartData)]
        public void MaybeXSelectMany3(string v)        => base.MaybeXSelectMany3Inner(v);
        #endregion
    }
}
