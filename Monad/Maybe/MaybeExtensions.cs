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

namespace PGSolutions.Utilities.Monads {
  /// <summary>Extension methods for Maybe&lt;T> to support LINQ "Comprehension" and "Fluent" syntax.</summary>
  /// <remarks>
  /// Unoptimized implementations of both Select() and SelectMany() have been retained
  /// as comments for documentation purposes, to emphasize the evolution to the
  /// optimized forms currently in use.
  /// 
  /// The intent is also to use this class as a tutorial for the exposition on
  /// generating the ptimized forms from the standard Monad implementations.
  /// </remarks>
  [Pure]
  public static class MaybeExtensions {
    /// <summary>Optimized LINQ-compatible implementation of Map.</summary>
    ///<remarks>Always available from Bind():
    ///         return @this.Bind(v => projector(v).ToMaybe());
    ///</remarks>
    public static Maybe<TResult>          Select<T,TResult>(this Maybe<T> @this,
      Func<T,TResult> projector
    ) {
      projector.ContractedNotNull("projector");
      return @this.Map(projector);
    }

    /// <summary>LINQ-compatible implementation of Bind.</summary>
    public static Maybe<TResult>          SelectMany<T, TResult>(this Maybe<T> @this,
      Func<T, Maybe<TResult>> selector
    ) {
      selector.ContractedNotNull("selector");
      return @this.Bind(selector);
    }

    /// <summary>LINQ-compatible implementation of Flatten.</summary>
    public static Maybe<TResult>          SelectMany<T, TSelection, TResult>(this Maybe<T> @this,
      Func<T, Maybe<TSelection>> selector,
      Func<T,TSelection,TResult> projector
    ) {
      selector.ContractedNotNull("selector");
      projector.ContractedNotNull("projector");
      return @this.Flatten(selector,projector);
    }

    [Pure]
    public static Maybe<TResult> Cast<T, TResult>
      (this Maybe<T> @this)
      where T : class where TResult : class {
      return @this.Bind<TResult>( v => v as TResult );
    }

    ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
    public static Maybe<bool> AreEqual<T>(this Maybe<T> lhs, Maybe<T> rhs) =>
        from lv in lhs from rv in rhs select lv.Equals(rv);

    ///<summary>Tests value-inequality, returning <b>Nothing</b> if either value doesn't exist.</summary>
    public static Maybe<bool> AreUnequal<T>(this Maybe<T> lhs, Maybe<T> rhs) =>
        from lv in lhs from rv in rhs select ! lv.Equals(rv);
  }
}
