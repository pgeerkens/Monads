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
    /// <summary>Extension methods supporting LINQ-comprehension syntax for the maybe monad on structs: <see cref="Nullable{T}"/>.</summary>
    public static partial class NullableLinq {
        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ <i>let</i> clause and queries with a single FROM clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        public static TResult? Select<TValue, TResult>(this TValue? @this,
            Func<TValue, TResult> projector
        ) where TValue : struct where TResult : struct
            => @this.HasValue ? projector?.Invoke(@this.Value)
                              : null;

        ///<summary>The monadic Bind operation of type T to type MaybeX2{TResult}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        public static TResult? SelectMany<TValue, TResult>(this TValue? @this,
            Func<TValue, TResult?> selector
        ) where TValue : struct where TResult : struct
            => @this.HasValue ? selector?.Invoke(@this.Value)
                              : null;

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        public static TResult? SelectMany<TValue, T, TResult>(this TValue? @this,
            Func<TValue, T?> selector,
            Func<TValue, T, TResult> projector
        ) where TValue : struct where T : struct where TResult : struct
            => @this.HasValue ? selector?.Invoke(@this.Value).SelectMany(e => 
                                projector?.Invoke(@this.Value, e))
                              : null;

        /// <summary>TODO</summary>
        /// <typeparam name="TValue">The underlying type of the Nulable{}.</typeparam>
        /// <param name="this"></param>
        /// <param name="default"></param>
        [Obsolete("This method is a temporary work-around enabling consistent usage between Nullable<T> and Maybe<T> of the ?? operator.")]
        public static TValue BitwiseOr<TValue>(this TValue? @this, TValue @default) where TValue:struct
            => @this ?? @default;
    }
}
