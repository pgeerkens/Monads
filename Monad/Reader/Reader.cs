﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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
    /// <summary>TODO</summary>
    public delegate TValue Reader<in TEnvironment, out TValue>(TEnvironment environment);

    /// <summary>TODO</summary>
    public static class Reader {
        /// <summary>TODO</summary>
        public static Reader<TEnvironment, TValue> New<TEnvironment, TValue>(Reader<TEnvironment, TValue> func) 
            => func;

        /// <summary>TODO</summary>
        public static Reader<TEnvironment,TResult>    Bind<TEnvironment,TSource, TResult>( this
            Reader<TEnvironment, TSource> @this,
            Func<TSource, Reader<TEnvironment, TResult>> selector
        ) {
            @this.ContractedNotNull(nameof(@this));
            selector.ContractedNotNull(nameof(selector));

            return environment => selector(@this(environment))(environment);
        }

        /// <summary>TODO</summary>
        public static Reader<TEnvironment,TResult>    Flatten<TEnvironment,TSource,TSelector,TResult>( this
            Reader<TEnvironment, TSource> @this,
            Func<TSource, Reader<TEnvironment, TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector
        ) { 
            @this.ContractedNotNull(nameof(@this));
            selector.ContractedNotNull(nameof(selector));
            resultSelector.ContractedNotNull(nameof(resultSelector));

            return environment =>
                {
                    var sourceResult = @this(environment);
                    return resultSelector(sourceResult, selector(sourceResult)(environment));
                };
        }

        /// <summary>η: T -> Reader&lt;TEnvironment, T></summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static Reader<TEnvironment, T>         ToReader<TEnvironment, T>(this T value) {
            value.ContractedNotNull(nameof(value));

            return environment => value;
        }
    }
}
