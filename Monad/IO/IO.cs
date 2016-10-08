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
    /// <summary>THE IO monad, for encapsulating side-effects only visible externally.</summary>
    /// <typeparam name="TResult"></typeparam>
    public delegate TResult IO<out TResult>();

    /// <summary>THe LINQ-extension methods for <see cref="IO{DateTimeResult}"/>.</summary>
    [Pure]
    public static class IOLinq {
        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        public static IO<TResult>   Select<TSource,TResult>(this IO<TSource> @this,
            Func<TSource,TResult> projector
        ) =>
            @this!=null && projector!= null ? () => projector(@this())
                                            : Null<TResult>();

        /// <summary>The monadic bind operator; unused by LINQ</summary>
        public static IO<TResult>   SelectMany<TSource,TResult>(this IO<TSource> @this,
            Func<TSource,IO<TResult>> selector
        ) =>
            @this!=null && selector!=null ? () => selector(@this()).Invoke()
                                          : Null<TResult>();

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        public static IO<TResult>   SelectMany<TSource,T,TResult>(this IO<TSource> @this,
            Func<TSource, IO<T>> selector,
            Func<TSource,T,TResult> projector
        ) =>
            @this!=null && selector!=null && projector!=null
                 ? () => @this.Select(s => projector(s,selector(s)()) ).Invoke()
                 : Null<TResult>();

        /// <summary>A typed null, for convenience.</summary>
        private static IO<TResult>   Null<TResult>() => null;
    }
}

