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
using System.Diagnostics.Contracts;

namespace PGSolutions.Utilities.Monads {
    /// <summary>TODO</summary>
    public struct IO<TSource> : IEquatable<IO<TSource>> {
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IO<TSource> Empty { get { return _empty; } } readonly static IO<TSource> _empty = new IO<TSource>();

       /// <summary>Create a new instance of the class.</summary>
        public IO(Func<TSource> functor) : this() {
            functor.ContractedNotNull("source");
            Contract.Ensures(_functor != null);
            _functor = functor;
        }

        /// <summary>Invokes the internal functor, returning the result.</summary>
        public TSource Invoke() => _functor();  readonly Func<TSource> _functor;

        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        /// <remarks>
        /// Used to implement the LINQ <i>let</i> clause.
        /// </remarks>
        [Pure]
        public IO<TResult> Select<TResult>(
            Func<TSource, TResult> projector
        ) {
            projector.ContractedNotNull("projector");

            var functor = _functor;
            return new IO<TResult>(() => projector(functor()));
        }

        /// <summary>LINQ-compatible implementation of the monadic bind operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with a single <i>from</i> clause.
        /// </remarks>
        [Pure]
        public IO<TResult> SelectMany<TResult>(
            Func<TSource, IO<TResult>> selector
        ) {
            selector.ContractedNotNull("selector");

            var functor = _functor;
            return new IO<TResult>(selector(functor.Invoke()).Invoke);
        }

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        [Pure]
        public IO<TResult> SelectMany<T, TResult>(
            Func<TSource, IO<T>> selector,
            Func<TSource, T, TResult> projector
        ) {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");

            var functor = _functor;
            return new IO<TResult>(() => {
                var source = functor();
                return selector(source).Select(t => projector(source, t)).Invoke();
            } );
        }

        ///<summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Contract.Invariant(_functor != null);
        }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            var other = obj as IO<TSource>?;
            return other != null && Equals(other.Value);
        }

        /// <summary>Tests value-equality.</summary>
        [Pure]
        public bool Equals(IO<TSource> other) => _functor.Equals(other._functor);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => _functor.GetHashCode();

        /// <summary>Tests value-equality.</summary>
        [Pure]
        public static bool operator ==(IO<TSource> lhs, IO<TSource> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        [Pure]
        public static bool operator !=(IO<TSource> lhs, IO<TSource> rhs) => ! lhs.Equals(rhs);
        #endregion
    }

    [Pure]
    public static class IO {
        /// <summary>TODO</summary>
        public static IO<TSource> ToIO<TSource>( this Func<TSource> source) {
            source.ContractedNotNull("source");
            return new IO<TSource>(source);
        }
    }
}

