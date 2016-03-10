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
#if false
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  [Pure]
  public static class Functions {
      // Id is alias of DotNet.Category.Id().Invoke
      public static T Id<T>(T value) { return DotNet.Category.Id<T>().Invoke(value); }
      public static T True<T,U> (T t, U u) { return t; }
      public static U False<T,U>(T t, U u) { return u; }
  }

  //public struct Unit {
  //  public override int GetHashCode() { return 0; }
  //  [Pure]public override bool Equals(object obj) { 
  //    var other = obj as Unit?;
  //    return other!= null  &&  obj != null;
  //  }
  //}

  public interface ICategory<TCategory> where TCategory : ICategory<TCategory> {
      // o = (m2, m1) -> composition
      [Pure]
      IMorphism<TSource, TResult, TCategory> o<TSource, TMiddle, TResult>(
          IMorphism<TMiddle, TResult, TCategory> m2, IMorphism<TSource, TMiddle, TCategory> m1);

      [Pure]
      IMorphism<TObject, TObject, TCategory> Id<TObject>();
  }

  public interface IMorphism<in TSource, out TResult, out TCategory>
    where TCategory : ICategory<TCategory>
  {
      [Pure]
      TCategory Category { get; }

      [Pure]
      TResult Invoke(TSource source);
  }

  public class DotNet : ICategory<DotNet>
  {
      [Pure]
      public IMorphism<TObject, TObject, DotNet> Id<TObject> () {
        Contract.Ensures(Contract.Result<IMorphism<TObject, TObject, DotNet>>() != null);
        return new DotNetMorphism<TObject, TObject>(@object => @object);
      }

      [Pure]
      public IMorphism<TSource, TResult, DotNet> o<TSource, TMiddle, TResult>
          (IMorphism<TMiddle, TResult, DotNet> m2, IMorphism<TSource, TMiddle, DotNet> m1) {
        return
              new DotNetMorphism<TSource, TResult>(@object => m2.Invoke(m1.Invoke(@object)));
      }

      private DotNet()
      {
      }

      //public static DotNet Category {[Pure] get; } = new DotNet();
      public static DotNet Category { [Pure]get {
        Contract.Ensures(Contract.Result<DotNet>() != null);
        return new DotNet();}
      } //= new DotNet();
  }

  public class DotNetMorphism<TSource, TResult> : IMorphism<TSource, TResult, DotNet> {
      private readonly Func<TSource, TResult> _function;

      public DotNetMorphism(Func<TSource, TResult> function) {
          function.ContractedNotNull("function");
          this._function = function;
      }

      public DotNet Category {
          [Pure]get {return DotNet.Category;}
      }

      [Pure]
      public TResult Invoke(TSource source) { return this._function(source); }

    /// <summary>The invariants enforced by this type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]private void ObjectInvariant() {
      Contract.Invariant( _function != null );
    }
  }
}
#endif