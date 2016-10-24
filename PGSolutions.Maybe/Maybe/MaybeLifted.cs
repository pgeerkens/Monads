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

namespace PGSolutions.Monads {
    /// <summary>Extension methods supporting LINQ-comprehension syntax crossing between maybe monads on classes <see cref="X{T}"/> and structs <see cref="Nullable{T}"/> .</summary>
    public static class MaybeLifted {
        #region X<TValue> lifted to TResult?
        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static TResult?          SelectMany<TValue, TResult>(this X<TValue> @this,
            Func<TValue, TResult?> selector
        ) where TValue : class where TResult : struct =>
            @this.HasValue ? selector?.Invoke(@this.Value) ?? null
                           : null;

        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static TResult?          SelectMany<TValue, T, TResult>(this X<TValue> @this,
            Func<TValue, T?> selector,
            Func<TValue, T, TResult> projector
        ) where TValue : class where T : struct where TResult : struct =>
            @this.HasValue ? selector?.Invoke(@this.Value).SelectMany(e =>
                             projector?.Invoke(@this.Value, e)) ?? null
                           : null;
        #endregion
        #region TValue? lifted to X<TResult>
        ///<summary>The monadic Bind operation of type T to type MaybeX2{TResult}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        public static X<TResult>    SelectMany<TValue, TResult>(this TValue? @this,
            Func<TValue, X<TResult>> selector
        ) where TValue : struct where TResult : class
            => @this.HasValue ? selector?.Invoke(@this.Value) ?? null
                              : null;

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        public static X<TResult>    SelectMany<TValue, T, TResult>(this TValue? @this,
            Func<TValue, X<T>> selector,
            Func<TValue, T, TResult> projector
        ) where TValue : struct where T : class where TResult : class
            => @this.HasValue ? selector?.Invoke(@this.Value).Select(e => 
                                projector?.Invoke(@this.Value, e)) ?? null
                              : null;
        #endregion
    }
}
