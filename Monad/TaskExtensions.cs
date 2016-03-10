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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PGSolutions.Utilities.Monads {
  // Impure.
  public static class TaskExtensions
  {
    public static async Task<TResult>     Select<TSource, TResult>(this
        Task<TSource> source, 
        Func<TSource, TResult> selector
    ) {
        return selector(await source);
    }
    // Select: (TSource -> TResult) -> (Task<TSource> -> Task<TResult>)
    public static Task<TResult>           Select2<TSource, TResult>(this
        Task<TSource> source,
        Func<TSource, TResult> selector
    ) { 
        return source.SelectMany(value => selector(value).Task());
    }

    // Not required, just for convenience.
    public static Task<TResult>           SelectMany<TSource, TResult>(this
        Task<TSource> source,
        Func<TSource, Task<TResult>> selector
    ) {
      return source.SelectMany(selector, Functions.Second);
    }

    // Required by LINQ.
    public static async Task<TResult>     SelectMany<TSource, TSelector, TResult>(this
        Task<TSource> source,
        Func<TSource, Task<TSelector>> selector,
        Func<TSource, TSelector, TResult> resultSelector
    ) {
        return resultSelector(await source, await selector(await source));
    }




    // Select: (Unit -> TResult) -> (Task -> Task<TResult>)
    public static Task<TResult>           Select<TResult>(this
        Task source,
        Func<Unit, TResult> selector
    ) {
        return source.SelectMany(value => selector(value).Task());
    }

    // Not required, just for convenience.
    public static Task<TResult>           SelectMany<TResult>(this
        Task source,
        Func<Unit, Task<TResult>> selector
    ) {
        return source.SelectMany(selector, Functions.Second);
    }

    // Required by LINQ.
    public static async Task<TResult>     SelectMany<TSelector, TResult>(
        this Task source,
        Func<Unit, Task<TSelector>> selector,
        Func<Unit, TSelector, TResult> resultSelector)
    {
        await source;
        return resultSelector(Unit._, await selector(Unit._));
    }

    // η: Unit -> Task.
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "unit")]
    public static Task Task(Unit unit) { return System.Threading.Tasks.Task.Run(() => { }); }

    // ι: TUnit -> Task is already implemented previously with η: Unit -> Task.

    // ι: Unit -> Func<Unit>
//    public static Task<Unit> Unit(Unit unit) { return unit.Task(); }
    public static Task<T> Task<T>(this T value) {
      return System.Threading.Tasks.Task.FromResult(value);
    }

    //// μ: Task<Task<T> => Task<T>
    //public static Task<TResult> Flatten<TResult>(this
    //    Task<Task<TResult>> source
    //) { 
    //    return source.SelectMany(Functions.Id);
    //}

    // η: T -> Task<T> is already implemented previously as TaskExtensions.Task.

    //// φ: Lazy<Task<T1>, Task<T2>> => Task<Lazy<T1, T2>>
    //public static Task<Lazy<T1, T2>> Binary2<T1, T2>
    //    (this Lazy<Task<T1>, Task<T2>> binaryFunctor) { 
    //    return 
    //        binaryFunctor.Value1.SelectMany(
    //            value1 => binaryFunctor.Value2,
    //            (value1, value2) => new Lazy<T1, T2>(value1, value2));
    //}

    // ι: TUnit -> Task<TUnit> is already implemented previously with η: T -> Task<T>.
  }
}
