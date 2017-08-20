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
    /// <summary>TODO</summary>
    public static class ReaderLinq {
        // Select: (TSource -> TResult) -> (Reader<TEnvironment, TSource> -> Reader<TEnvironment, TResult>)
        /// <summary>Select: (TSource -> TResult) -> (Reader&lt;TEnvironment, TSource> -> Reader&lt;TEnvironment, TResult>)</summary>
        public static Reader<TEnvironment, TResult>   Select<TEnvironment, TSource, TResult>(this 
            Reader<TEnvironment, TSource> source,
            Func<TSource, TResult> selector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));

            return source.SelectMany(value => selector(value).ToReader<TEnvironment, TResult>());
        }

        /// <summary>TODO</summary>
        public static Reader<TEnvironment,TResult>    SelectMany<TEnvironment, TSource, TResult>(this
            Reader<TEnvironment, TSource> source,
            Func<TSource, Reader<TEnvironment, TResult>> selector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));

            return environment => selector(source(environment))(environment);
        }

        /// <summary>TODO</summary>
        public static Reader<TEnvironment,TResult>    SelectMany<TEnvironment,TSource,TSelector,TResult>(this
            Reader<TEnvironment, TSource> source,
            Func<TSource, Reader<TEnvironment, TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector
        ) { 
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));
            resultSelector.ContractedNotNull(nameof(resultSelector));

            return environment =>
                {
                    var sourceResult = source(environment);
                    return resultSelector(sourceResult, selector(sourceResult)(environment));
                };
        }

        //// φ: Lazy<Reader<TEnvironment, T1>, Reader<TEnvironment, T2>> => Reader<TEnvironment, Lazy<T1, T2>>
        //public static Reader<TEnvironment, Lazy<T1, T2>> Binary<TEnvironment, T1, T2>
        //    (this Lazy<Reader<TEnvironment, T1>, Reader<TEnvironment, T2>> binaryFunctor) {
        //    return
        //        binaryFunctor.Value1.SelectMany(
        //            value1 => binaryFunctor.Value2,
        //            (value1, value2) => new Lazy&lt;T1, T2>(value1, value2));
        //}

        /// <summary>ι: TUnit -> Reader&lt;TEnvironment, TUnit></summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Reader<TEnvironment,Unit>       Unit<TEnvironment>(Unit u)
        => u.ToReader<TEnvironment, Unit>();
    }
}
