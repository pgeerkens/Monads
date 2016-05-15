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
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  public delegate TValue Reader<in TEnvironment, out TValue>(TEnvironment environment);

  public static class Reader {
    public static Reader<TEnvironment,TResult>    Bind<TEnvironment,TSource, TResult>( this
        Reader<TEnvironment, TSource> @this,
        Func<TSource, Reader<TEnvironment, TResult>> selector
    ) {
        @this.ContractedNotNull("this");
        selector.ContractedNotNull("selector");
        Contract.Ensures(Contract.Result<Reader<TEnvironment,TResult> >() != null);

        return environment => selector(@this(environment))(environment);
    }

    // Required by LINQ.
    public static Reader<TEnvironment,TResult>    Flatten<TEnvironment,TSource,TSelector,TResult>( this
        Reader<TEnvironment, TSource> @this,
        Func<TSource, Reader<TEnvironment, TSelector>> selector,
        Func<TSource, TSelector, TResult> resultSelector
    ) { 
        @this.ContractedNotNull("this");
        selector.ContractedNotNull("selector");
        resultSelector.ContractedNotNull("resultSelector");
        Contract.Ensures(Contract.Result<Reader<TEnvironment,TResult> >() != null);

        return environment =>
            {
                var sourceResult = @this(environment);
                return resultSelector(sourceResult, selector(sourceResult)(environment));
            };
    }

    /// <summary>η: T -> Reader<TEnvironment, T></summary>
    /// <typeparam name="TEnvironment"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    public static Reader<TEnvironment, T>         ToReader<TEnvironment, T>(this T value) {
        value.ContractedNotNull("value");
        Contract.Ensures(Contract.Result<Reader<TEnvironment, T> >() != null);

        return environment => value;
    }
  }

  [Pure]
  public static class ReaderLinq {
    // Select: (TSource -> TResult) -> (Reader<TEnvironment, TSource> -> Reader<TEnvironment, TResult>)
    public static Reader<TEnvironment, TResult>   Select<TEnvironment, TSource, TResult>(this 
        Reader<TEnvironment, TSource> source,
        Func<TSource, TResult> selector
    ) {
        source.ContractedNotNull("source");
        selector.ContractedNotNull("selector");
        Contract.Ensures(Contract.Result<Reader<TEnvironment,TResult>>() != null);

        return source.SelectMany(value => selector(value).ToReader<TEnvironment, TResult>());
    }

    // Not required, just for convenience.
    public static Reader<TEnvironment,TResult>    SelectMany<TEnvironment, TSource, TResult>(this
        Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TResult>> selector
    ) {
        source.ContractedNotNull("source");
        selector.ContractedNotNull("selector");
        Contract.Ensures(Contract.Result<Reader<TEnvironment,TResult> >() != null);

        return environment => selector(source(environment))(environment);
    }

    // Required by LINQ.
    public static Reader<TEnvironment,TResult>    SelectMany<TEnvironment,TSource,TSelector,TResult>(this
        Reader<TEnvironment, TSource> source,
        Func<TSource, Reader<TEnvironment, TSelector>> selector,
        Func<TSource, TSelector, TResult> resultSelector
    ) { 
        source.ContractedNotNull("source");
        selector.ContractedNotNull("selector");
        resultSelector.ContractedNotNull("resultSelector");
        Contract.Ensures(Contract.Result<Reader<TEnvironment,TResult> >() != null);

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
    //            (value1, value2) => new Lazy<T1, T2>(value1, value2));
    //}

    /// <summary>ι: TUnit -> Reader<TEnvironment, TUnit></summary>
    /// <typeparam name="TEnvironment"></typeparam>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static Reader<TEnvironment,Unit>       Unit<TEnvironment>(Unit unit)
    {
        Contract.Ensures(Contract.Result<Reader<TEnvironment,Unit>>() != null);

        return unit.ToReader<TEnvironment, Unit>();
    }
  }

  [Pure]
  public static class Functions {
      public static TFirst        First<TFirst,TSecond> (TFirst first, TSecond second) {
          first.ContractedNotNull("first");
          second.ContractedNotNull("second");
          Contract.Ensures(Contract.Result<TFirst>() != null);

          return first;
      }

      public static TSecond       Second<TFirst,TSecond>(TFirst first, TSecond second) {
          first.ContractedNotNull("first");
          second.ContractedNotNull("second");
          Contract.Ensures(Contract.Result<TSecond>() != null);

          return second;
      }
  }
}
