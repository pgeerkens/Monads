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
using System.Diagnostics.Contracts;

namespace PGSolutions.Monads {
    /// <summary>Extension methods supporting LINQ-comprehension syntax for the maybe monad on classes: <see cref="X{T}"/>.</summary>
    [Pure]
    public static class MaybeLinq {
        /// <summary>LINQ-ible implementation of the monadic map operator.</summary>
        ///<remarks>Used by the LINQ <i>let</i> clause and queries with a single FROM clause.</remarks>
        public static X<TResult>    Select<TValue, TResult>(this X<TValue> @this,
            Func<TValue, TResult> projector
        ) where TValue : class where TResult : class =>
            @this.HasValue ? projector?.Invoke(@this.Value) : null;

        ///<summary>The monadic Bind operation of type <typeparamref name="TValue"/> to <typeparamref name="TResult"/>.</summary>
        ///<remarks>Convenient; but not used by LINQ.</remarks>
        public static X<TResult>    SelectMany<TValue, TResult>(this X<TValue> @this,
            Func<TValue, X<TResult>> selector
        ) where TValue : class where TResult : class =>
            @this.HasValue ? selector?.Invoke(@this.Value) ?? null
                           : null;

        /// <summary>LINQ-ible implementation of the monadic join operator.</summary>
        /// <remarks>Used by LINQ queries with multiple <i>from</i> clauses.</remarks>
        public static X<TResult>    SelectMany<TValue, T, TResult>(this X<TValue> @this,
            Func<TValue, X<T>> selector,
            Func<TValue, T, TResult> projector
        ) where TValue : class where T : class where TResult: class =>
            @this.HasValue ? selector?.Invoke(@this.Value).Select(e =>
                             projector?.Invoke(@this.Value, e)) ?? null
                           : null;

        /// <summary>LINQ-ible Cast implementation. Argument is "boxed" if not already a class object.</summary>
        public static X<T> Cast<T>(this X<object> @this) where T: class => from obj in @this select (T)obj;

        /// <summary>LINQ-ible Cast implementation for a class object.</summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static TResult Cast<TValue,TResult>(this TValue @this) where TValue:TResult 
            => @this != null ? @this : default(TResult);
    }
}
