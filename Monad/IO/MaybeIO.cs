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
using System.Linq;

namespace PGSolutions.Monads {
    /// <summary>TODO</summary>
    public static class MaybeIO {
         #region X<TValue> lifted to IO<TResult>
        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static IO<TResult>       SelectMany<TValue, TResult>(this X<TValue> @this,
            Func<TValue, IO<TResult>> selector
        ) where TValue : class where TResult : struct =>
            ( from t in @this select selector?.Invoke(t) ?? null ) | null;

        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static IO<TResult>       SelectMany<TValue, T, TResult>(this X<TValue> @this,
            Func<TValue, IO<T>> selector,
            Func<TValue, T, TResult> projector
        ) where TValue : class where T : struct where TResult : struct =>
            ( from t in @this select selector?.Invoke(t).SelectMany(e =>
                                         projector?.Invoke(t, e).ToIO() ?? null
                                     ) ?? null)
            | null;
            //@this.HasValue ? selector?.Invoke(@this.Value).SelectMany(e =>
            //                     projector?.Invoke(@this.Value, e).ToIO() ?? null
            //                 ) ?? null
            //               : null;
        #endregion
        //================================================================================
        #region IO<TSource> lifted to X<TResult>
        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static X<TResult>        SelectMany<TValue, TResult>(this IO<TValue> @this,
            Func<TValue, X<TResult>> selector
        ) where TResult : class =>
            @this!=null ? selector?.Invoke(@this.Invoke()) ?? default(TResult)
                        : null;

        ///<summary>The monadic Bind operation of class-type <typeparamref name="TValue"/> to struct-type <typeparamref name="TResult"/>.</summary>
        public static X<TResult>        SelectMany<TValue, T, TResult>(this IO<TValue> @this,
            Func<TValue, X<T>> selector,
            Func<TValue, T, TResult> projector
        ) where T : class where TResult : class =>
            @this!=null ? selector?.Invoke(@this.Invoke()).SelectMany(e =>
                          projector?.Invoke(@this.Invoke(), e).AsX()) ?? null
                        : null;
        #endregion
   }
}
