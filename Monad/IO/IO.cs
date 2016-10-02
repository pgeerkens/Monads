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

namespace PGSolutions.Monads {
    /// <summary>TODO</summary>
    public struct IO<TSource> : IEquatable<IO<TSource>> {
        /// <summary>Create a new instance of the class.</summary>
        public IO(Func<TSource> functor) : this() { _functor = functor; }

        /// <summary>Invokes the internal functor, returning the result.</summary>
        public TSource Invoke() => (_functor | Default)();

        /// <summary>Returns true exactly when the contained functor is not null.</summary>
        public bool HasValue => _functor != null;

        X<Func<TSource>> _functor { get; }

        static Func<TSource> Default => ()=>default(TSource);

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]public override bool Equals(object obj) => (obj as IO<TSource>?)?.Equals(this) ?? false;

        /// <summary>Tests value-equality.</summary>
        [Pure]public bool Equals(IO<TSource> other) => _functor.Equals(other._functor);

        /// <summary>Tests value-equality.</summary>
        [Pure]public static bool operator ==(IO<TSource> lhs, IO<TSource> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        [Pure]public static bool operator !=(IO<TSource> lhs, IO<TSource> rhs) => ! lhs.Equals(rhs);

        /// <inheritdoc/>
        [Pure]public override int GetHashCode() => _functor.GetHashCode();
        #endregion
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    [Pure]
    public static class IO {
        /// <summary>TODO</summary>
        public static IO<TSource> ToIO<TSource>( this Func<TSource> source) {
            source.ContractedNotNull(nameof(source));
            return new IO<TSource>(source);
        }

        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        /// <remarks>
        /// Used to implement the LINQ <i>let</i> clause.
        /// </remarks>
        [Pure]
        public static IO<TResult> Select<TSource,TResult>(this IO<TSource> @this,
            Func<TSource,TResult> projector
        ) =>
            @this.HasValue && projector!=null
                 ? New(() => projector(@this.Invoke()))
                 : Null<TResult>();

        /// <summary>LINQ-compatible implementation of the monadic bind operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with a single <i>from</i> clause.
        /// </remarks>
        [Pure]
        public static IO<TResult> SelectMany<TSource,TResult>(this IO<TSource> @this,
            Func<TSource,IO<TResult>> selector
        ) =>
            @this.HasValue && selector!=null
                 ? New(() => selector(@this.Invoke()).Invoke())
                 : Null<TResult>();

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        [Pure]
        public static IO<TResult> SelectMany<TSource,T,TResult>(this IO<TSource> @this,
            Func<TSource, IO<T>> selector,
            Func<TSource,T,TResult> projector
        ) =>
            @this.HasValue && selector!=null && projector!=null
                 ? New(() => { var s = @this.Invoke(); return projector(s, selector(s).Invoke()); } )
                 : Null<TResult>();

        /// <summary>COnvenince factory method to provide type inference.</summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="functor"></param>
        /// <returns></returns>
        public static IO<TResult> New<TResult> (Func<TResult> functor) => new IO<TResult>(functor);

        private static IO<TResult> Null<TResult>() => new IO<TResult>(null);
    }
}

