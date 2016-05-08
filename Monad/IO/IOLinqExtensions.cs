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

namespace PGSolutions.Utilities.Monads {

    [ContractClass(typeof(IMonadContract<>))]
    public interface IMonad<out TSource> {
        IMonad<TResult> Select<TResult>(
            Func<TSource, TResult> selector
        );

        IMonad<TResult> SelectMany<TResult>(
            Func<TSource, IMonad<TResult>> selector
        );

        IMonad<TResult> SelectMany<T, TResult>(
            Func<TSource, IMonad<T>> selector,
            Func<TSource, T, TResult> resultSelector
        );

        TSource Invoke();
    }

    [ContractClassFor(typeof(IMonad<>))]
    public abstract class IMonadContract<TSource> : IMonad<TSource> {
        private IMonadContract() { /* NO-OP */ }

        [Pure]
        public IMonad<TResult> Select<TResult>(
            Func<TSource, TResult> selector
        ) {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);
            return default(IMonad<TResult>);
        }

        [Pure]
        public IMonad<TResult> SelectMany<TResult>(
            Func<TSource, IMonad<TResult>> selector
        ) {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);
            return default(IMonad<TResult>);
        }

        [Pure]
        public IMonad<TResult> SelectMany<T, TResult>(
            Func<TSource, IMonad<T>> selector,
            Func<TSource, T, TResult> resultSelector
        ) {
            selector.ContractedNotNull("selector");
            resultSelector.ContractedNotNull("resultSelector");
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);
            return default(IMonad<TResult>);
        }
        public TSource Invoke() { return default(TSource); }
    }

    public struct IO<TSource> : IMonad<TSource> {
        public IO(Func<TSource> functor) : this() {
            functor.ContractedNotNull("source");
            Contract.Ensures(_functor != null);
            _functor = functor;
        }

        private Func<TSource> _functor;
        public TSource Invoke() { return _functor(); }

        public static IO<TSource> ToIO2(Func<TSource> source) {
            return new IO<TSource>(source);
        }
        public static implicit operator IO<TSource>(Func<TSource> source) {
            return new IO<TSource>(source);
        }

        ///<summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Contract.Invariant(_functor != null);
        }

        /// <inheritdoc/>
        [Pure]
        public IMonad<TResult> Select<TResult>(
            Func<TSource, TResult> selector
        ) {
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);

            var functor = _functor;
            return new IO<TResult>(() => selector(functor()));
        }

        /// <inheritdoc/>
        [Pure]
        public IMonad<TResult> SelectMany<TResult>(
            Func<TSource, IMonad<TResult>> selector
        ) {
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);

            var functor = _functor;
            return new IO<TResult>(() => selector(functor.Invoke()).Invoke());
        }

        /// <inheritdoc/>
        [Pure]
        public IMonad<TResult> SelectMany<T, TResult>(
            Func<TSource, IMonad<T>> selector, 
            Func<TSource, T, TResult> resultSelector
        ) {
            Contract.Ensures(Contract.Result<IMonad<TResult>>() != null);

            var functor = _functor;
            return new IO<TResult>(() => {
                var source = functor();
                return selector(source).Select(t => resultSelector(source, t)).Invoke();
            });
        }
    }
}

