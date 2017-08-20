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
using System.Threading.Tasks;

namespace PGSolutions.Monads {
    using static Task;
    using static TaskContinuationOptions;
    using static Functions;

    /// <summary>LINQ extension methods for <see cref="System.Threading.Tasks.Task"/> and 
    ///          <see cref="System.Threading.Tasks.Task{TResult}"/>.
    /// </summary>
    /// <remarks>IMpure.</remarks>
    public static partial class TaskExtensionsAsync {
        /// <summary>TODO</summary>
        public static Task<TResult>         Select<TSource, TResult>(this Task<TSource> source,
            Func<TSource, TResult> selector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));

            return source.Flatten(value => selector(value).Task(), Second) | DefaultTask<TResult>();
        }

        /// <summary>TODO</summary>
        public static Task<TResult>         SelectMany<TSource, TResult>(this Task<TSource> source,
            Func<TSource, Task<TResult>> selector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));

            return source.Flatten(selector, Second) | DefaultTask<TResult>();
        }

        /// <summary>TODO</summary>
        public static Task<TResult>         SelectMany<TSource, T, TResult>(this Task<TSource> source,
            Func<TSource, Task<T>> selector,
            Func<TSource, T, TResult> resultSelector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));
            resultSelector.ContractedNotNull(nameof(resultSelector));

            return source.Flatten(selector,resultSelector) | DefaultTask<TResult>();
        }
        /// <summary>μ: Task{Task{T}} => Task{T}</summary>
        public static Task<TResult>         FlatMap<TResult>(this Task<Task<TResult>> source
        ) {
            source.ContractedNotNull(nameof(source));

            return source.Flatten(Functions.Identity, Second) | DefaultTask<TResult>();
        }

        /// <summary>TODO</summary>
        public static Task<Unit>            ToTaskUnit(this Task source) {
            source.ContractedNotNull(nameof(source));

            return source.Flatten(value => value.Task(), Second) | DefaultTask<Unit>();
        }

        /// <summary>TODO</summary>
        private static X<Task<TResult>>     Flatten<TSource, T, TResult>(this Task<TSource> source,
            Func<TSource, Task<T>> selector,
            Func<TSource, T, TResult> resultSelector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));
            resultSelector.ContractedNotNull(nameof(resultSelector));

            return source.ContinueWith(u => selector(u.Result)
                         .ContinueWith(v => resultSelector(u.Result, v.Result), NotOnCanceled)
                                                                       .Result, NotOnCanceled);
        }

        /// <summary>TODO</summary>
        private static X<Task<TResult>>     Flatten<T, TResult>(this Task source,
            Func<Unit, Task<T>> selector,
            Func<Unit, T, TResult> resultSelector
        ) {
            source.ContractedNotNull(nameof(source));
            selector.ContractedNotNull(nameof(selector));
            resultSelector.ContractedNotNull(nameof(resultSelector));

            return source.ContinueWith(u => selector(Unit.Empty)
                         .ContinueWith(v => resultSelector(Unit.Empty, v.Result), NotOnCanceled)
                                                                     .Result, NotOnCanceled);
        }

        /// <summary>TODO</summary>
        private static Task<TResult>        DefaultTask<TResult>() {
            return Task(default(TResult));
        }

        /// <summary>η: T -> Task{T}</summary>
        private static Task<TResult>        Task<TResult>(this TResult value) {
            var task = FromResult(value);
            return task;
        }

        //// φ: Lazy<Task<T1>, Task<T2>> => Task<Lazy<T1, T2>>
        //public static Task<Lazy<T1, T2>> Binary2<T1, T2>
        //    (this Lazy<Task<T1>, Task<T2>> binaryFunctor) { 
        //    return 
        //        binaryFunctor.Value1.SelectMany(
        //            value1 => binaryFunctor.Value2,
        //            (value1, value2) => new Lazy<T1, T2>(value1, value2));
        //}
    }
}
