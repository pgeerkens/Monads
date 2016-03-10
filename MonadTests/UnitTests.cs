using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;
using MsTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGSolutions.Utilities.Monads.UnitTests {

  [MsTest.TestClass]
  public partial class EnumerableTests {

    [Fact][MsTest.TestMethod]
    public void ExecutionTest() {
      bool isExecuted1 = false;
      IEnumerable<int> enumerable1 = new int[] { 0, 1 };
      IEnumerable<int> enumerable2 = new int[] { 1, 2 };
      Func<int, Func<int, int>> f = x => y => { isExecuted1 = true; return x + y; };
      IEnumerable<int> query1 = from x in enumerable1
                                from y in enumerable2
                                let z = f(x)(y)
                                where z > 1
                                select z;
      Assert.False(isExecuted1, "isExecuted: laziness");
      Assert.Equal(query1.ToList(), new List<int> { 2, 2, 3 }); //, "Execution");
      Assert.True(isExecuted1, "isExecuted: execution");

    }

    static readonly Func<int, IEnumerable<int>> _addOne = x => (x + 1).Enumerable();

      /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
    [Fact][MsTest.TestMethod]
    public void MonadLawsTest1() {
      // Monad law 1: m.Monad().SelectMany(f) == f(m)
      var actual1 = 1.Enumerable().SelectMany(_addOne).ToList();
      Assert.NotNull(actual1);
      var a = _addOne(1); Contract.Assume(a != null);
      var expected1 = a.ToList();
      Assert.Equal(expected1, actual1);
    }
      /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
    [Fact][MsTest.TestMethod]
    public void MonadLawsTest2() {
      IEnumerable<int> enumerable1 = new int[] { 0, 1 };

      // Monad law 2: M.SelectMany(Monad) == M
      var actual2   = enumerable1.SelectMany(EnumerableExtensions.Enumerable).ToList();
      var expected2 = enumerable1.ToList();
      Assert.Equal(actual2, expected2);
    }
      /// <summary>Monad law 3: M.Bind(f).Bind(g) == M.Bind(x => f(x).Bind(g))</summary>
    [Fact][MsTest.TestMethod]
    public void MonadLawsTest3() {
      IEnumerable<int> enumerable2 = new int[] { 2, 4 };

      // Monad law 3: M.SelectMany(f1).SelectMany(f2) == M.SelectMany(x => f1(x).SelectMany(f2))
      Func<int, IEnumerable<int>> addTwo = x => (x + 2).Enumerable();
      var actual3   = enumerable2.SelectMany(_addOne).SelectMany(addTwo).ToList();
      var expected3 = enumerable2.SelectMany(x => _addOne(x).SelectMany(addTwo)).ToList();
      Assert.Equal(actual3, expected3);
    }
  }


  //public static class Enumerable
  //{
  //    [Pure]
  //    public static IEnumerable<TResult> SelectMany2<TSource, TSelector, TResult>(
  //        this IEnumerable<TSource> source,
  //        Func<TSource, IEnumerable<TSelector>> selector,
  //        Func<TSource, TSelector, TResult> resultSelector)
  //    {
  //        Contract.Ensures(Contract.Result<IEnumerable<TResult>>() != null);
  //        foreach (TSource sourceItem in source)
  //        {
  //            foreach (TSelector selectorItem in selector(sourceItem))
  //            {
  //                yield return resultSelector(sourceItem, selectorItem);
  //            }
  //        }
  //    }
  //}
  //[Pure]
  //public static partial class EnumerableExtensions {
  //    public static IEnumerable<T> Enumerable<T>(this T value) {
  //      Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
  //      yield return value;
  //    }
  ////    public static IEnumerable<TSource> Flatten<TSource>(this IEnumerable<IEnumerable<TSource>> source)
  ////    {
  ////        foreach (IEnumerable<TSource> enumerable in source)
  ////        {
  ////            foreach (TSource value in enumerable)
  ////            {
  ////                yield return value;
  ////            }
  ////        }
  ////    }

  //    //public static IEnumerable<TResult> Select3<TSource, TResult>
  //    //    (this IEnumerable<TSource> source, Func<TSource, TResult> selector) {
  //    //      return
  //    //            from sourceValue in source
  //    //            from selectorValue in selector(sourceValue).Enumerable()
  //    //            select selectorValue;
  //    //}

  ////    public static IEnumerable<TSource> Flatten3<TSource>
  ////        (this IEnumerable<IEnumerable<TSource>> source) {
  ////      return
  ////        from enumerable in source
  ////        from value in enumerable
  ////        select value;
  ////    }
  //}
}
