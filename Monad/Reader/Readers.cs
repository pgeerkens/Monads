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

using PGSolutions.Utilities.Monads;

namespace PGSolutions.Utilities.Monads {
  /// <summary>Sample instances of the <see cref="Reader"/> monad pattern.</summary>
  public static class Readers {
    /// <summary>An auto-incrementing zero-baed counter.</summary>
    public static Func<int>             Counter() {
        Contract.Ensures(Contract.Result<Func<int>>() != null);
        return Counter(0);
    }

    /// <summary>An auto-incrementing <i>start</i>-based counter.</summary>
    /// <param name="start">The initial value of the counter.</param>
    public static Func<int>             Counter(int start) {
        Contract.Ensures(Contract.Result<Func<int>>() != null);

        var index   = start;
        var reader  = new Reader<int,int>(_ => index++);
        return ()   => reader(int.MinValue);
    }

    /// <summary>An auto-incrementing zero-based counter that reports when <paramref name="predicate"/> is true of the counter.</summary>
    /// <param name="predicate">The condition for when <b>true</b> should be reported.</param>
    public static Func<bool>            MatchCounter(Func<int, bool> predicate) {
        predicate.ContractedNotNull("predicate");
        Contract.Ensures(Contract.Result<Func<bool>>() != null);
        return MatchCounter(predicate,0);
    }

    /// <summary>An auto-incrementing zero-based counter that reports when <paramref name="predicate"/> is true of the counter.</summary>
    /// <param name="predicate">The condition for when <b>true</b> should be reported.</param>
    /// <param name="start">The initial value of the counter.</param>
    public static Func<bool>            MatchCounter(Func<int, bool> predicate, int start) {
        predicate.ContractedNotNull("predicate");
        Contract.Ensures(Contract.Result<Func<bool>>() != null);

        var index   = start;
        var reader  = new Reader<int, bool>(_ => predicate(index++));
        return ()   => reader(int.MinValue);
    }

    /// <summary>A timer that, on each invocation, reports the <see cref="TimeSpan"/> since its creation.</summary>
    public static Func<TimeSpan>        Timer() {
        Contract.Ensures(Contract.Result<Func<TimeSpan>>() != null);

        var oldTime = DateTime.Now;
        var timer   = new Reader<DateTime,TimeSpan>(newTime => newTime - oldTime);
        return ()   => timer(DateTime.Now);
    }

    #region  What is Timer() really doing?
    public static Func<TimeSpan> Timer2() {
        Contract.Ensures(Contract.Result<Func<TimeSpan>>() != null);

        return new _TimerInternals_().Invoke;
    }
    private class _TimerInternals_ {
      public TimeSpan Invoke() { return _timer(DateTime.Now); }

      internal _TimerInternals_() {
          _oldTime  = DateTime.Now;
          _timer    = new Reader<DateTime,TimeSpan>(newTime => newTime - _oldTime);
      }
      private  readonly DateTime                  _oldTime;
      private  readonly Reader<DateTime,TimeSpan> _timer;

      /// <summary>The invariants enforced by this struct type.</summary>
      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
      [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ContractInvariantMethod]
      [Pure]private void ObjectInvariant() {
          Contract.Invariant(_timer != null);
      }
    }
    #endregion
  }
}
