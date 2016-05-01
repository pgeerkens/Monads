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
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads.UnitTests {
  internal static class MaybeTestsExtensions {
    /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
    public static string ToNothingString<T>(this Maybe<T> @this) {
      Contract.Ensures(Contract.Result<string>() != null);
      return @this.SelectMany<string>(v => v.ToString()) | "Nothing";
    }

    /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
    public static string ToNothingString<T>(this MaybeX<T> @this
    ) where T:class {
      Contract.Ensures(Contract.Result<string>() != null);
      return @this.SelectMany<string>(v => v.ToString()) | "Nothing";
    }
  }

  internal static class Enumerable
  {
      //[Pure]
      //public static IEnumerable<TResult> SelectMany2<TSource, TSelector, TResult>(
      //    this IEnumerable<TSource> source,
      //    Func<TSource, IEnumerable<TSelector>> selector,
      //    Func<TSource, TSelector, TResult> resultSelector)
      //{
      //    Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);
      //    foreach (TSource sourceItem in source)
      //    {
      //        foreach (TSelector selectorItem in selector(sourceItem))
      //        {
      //            yield return resultSelector(sourceItem, selectorItem);
      //        }
      //    }
      //}
  }

  [Pure]
  internal static partial class EnumerableExtensions {
      public static IEnumerable<T> Enumerable<T>(this T value) {
        Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
        yield return value;
      }
  //    public static IEnumerable<TSource> Flatten<TSource>(this IEnumerable<IEnumerable<TSource>> source)
  //    {
  //        foreach (IEnumerable<TSource> enumerable in source)
  //        {
  //            foreach (TSource value in enumerable)
  //            {
  //                yield return value;
  //            }
  //        }
  //    }

      //public static IEnumerable<TResult> Select3<TSource, TResult>
      //    (this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
      //      return
      //            from sourceValue in source
      //            from selectorValue in selector(sourceValue).Enumerable()
      //            select selectorValue;
      //}

  //    public static IEnumerable<TSource> Flatten3<TSource>
  //        (this IEnumerable<IEnumerable<TSource>> source) {
  //      return
  //        from enumerable in source
  //        from value in enumerable
  //        select value;
  //    }
  }
}
