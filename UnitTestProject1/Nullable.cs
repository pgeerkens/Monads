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

namespace PGSolutions.Monads.Test {
    using static Contract;

    public static class MaybeNullableLinq {
        public static TResult?      Select<TValue,TResult>(this TValue? @this,
            Func<TValue,TResult>    projector
        ) where TValue:struct where TResult:struct 
            => @this.HasValue ? projector?.Invoke(@this.Value)
                              : null;

        private static TResult? SelectMany<TValue, TResult>(this TValue? @this,
            Func<TValue, TResult?> selector
        ) where TValue : struct where TResult : struct
            => @this.HasValue ? selector?.Invoke(@this.Value)
                              : null;

        public static TResult?      SelectMany<TValue,T,TResult>(this TValue? @this,
            Func<TValue,T?>         selector,
            Func<TValue,T,TResult>  projector
        ) where TValue:struct where T:struct where TResult:struct
            => @this.HasValue ? selector?.Invoke(@this.Value).SelectMany(e => projector?.Invoke(@this.Value,e))
                              : null;
        //-------------------------------------------------------------------------------------------------------
        public static TResult SelectMany<TValue, TResult>(this TValue? @this,
            Func<TValue, TResult> selector
        ) where TValue : struct where TResult : class
            => @this.HasValue ? selector?.Invoke(@this.Value) ?? null
                              : null;

        public static TResult       SelectMany<TValue,T,TResult>(this TValue? @this,
            Func<TValue,T>          selector,
            Func<TValue,T,TResult>  projector
        ) where TValue:struct where T:class where TResult:class
            => @this.HasValue ? selector?.Invoke(@this.Value).SelectMany(e => projector?.Invoke(@this.Value,e))
                              : null;
   // }

   //public static class MaybeClassLinq {
        public static TResult       Select<TValue,TResult>(this TValue @this,
            Func<TValue,TResult>    projector
        ) where TValue:class where TResult:class 
            => @this != null ? projector?.Invoke(@this)
                             : null;

        private static TResult SelectMany<TValue, TResult>(this TValue @this,
            Func<TValue, TResult> selector
        ) where TValue : class where TResult : class
            => @this != null ? selector?.Invoke(@this)
                             : null;

        public static TResult       SelectMany<TValue,T,TResult>(this TValue @this,
            Func<TValue,T>          selector,
            Func<TValue,T,TResult>  projector
        ) where TValue:class where T:class where TResult:class
            => @this != null ? selector?.Invoke(@this).SelectMany(e => projector?.Invoke(@this,e))
                             : null;
    //-------------------------------------------------------------------------------------------------------
        public static TResult?   SelectMany<TValue,T,TResult>(this TValue @this,
            Func<TValue,T?>         selector,
            Func<TValue,T,TResult>  projector
        ) where TValue:class where T:struct where TResult:struct
            => @this != null ? selector?.Invoke(@this).SelectMany(e => projector?.Invoke(@this,e))
                             : null;

        public static TResult Cast<TValue,TResult>(this TValue @this) where TValue:class,TResult 
            => @this != null ? @this : null;
    }
}
