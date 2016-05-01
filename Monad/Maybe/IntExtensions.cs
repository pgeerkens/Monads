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
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  public static class IntExtensions {
    public static Maybe<int> DoSomeDivision(this int @this, int denominator) {
      return from a in @this.Div(denominator)
             from b in a.Div(2)
             select b;
    }
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1",
      Justification="Limitation of Code Analysis - verification done by Code Contracts.")]
    public static Maybe<int> DoSomeDivision(this int @this, params int[] denominators) {
      denominators.ContractedNotNull("denominators");

      var intermediate = @this.ToMaybe();
      foreach (var d in denominators) intermediate = intermediate.SelectMany(v => v.Div(d));
      return intermediate;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Div")]
    public static Maybe<int> Div(this Maybe<int> numerator, Maybe<int> denominator) {
      return numerator.SelectMany(n => denominator.SelectMany(d => n.Div(d)));
    }
    public static Maybe<int> Add(this Maybe<int> numerator, Maybe<int> denominator) {
      return numerator.SelectMany(n => denominator.SelectMany(d => n.Add(d)));
    }
    public static Maybe<int> Sub(this Maybe<int> numerator, Maybe<int> denominator) {
      return numerator.SelectMany(n => denominator.SelectMany(d => n.Sub(d)));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Div")]
    public static Maybe<int> Div(this int numerator, int denominator) {
      return denominator == 0 || (numerator == Int32.MinValue && denominator == -1)
          ? Maybe<int>.Nothing
          : numerator / denominator;
    }
    /// <summary>
    /// courtesy Ivan Stoev: http://w3foverflow.com/question/integer-overflow-detection-c-for-add/
    /// </summary>
    /// <remarks>
    /// See also <href a="https://www.fefe.de/intof.html">Catching Integer Overflows in C</href>
    /// </remarks>
    public static Maybe<int> Add(this int a, int b) {
      unchecked {
        int c = a + b;
        return ((a ^ b) >= 0) & ((a ^ c) < 0) ? Maybe<int>.Nothing
                                              : c;
      }
    }
    public static Maybe<int> Sub(this int a, int b) {
      return b != int.MinValue ? a.Add(-b)
                       : a < 0 ? b.Add(a).SelectMany<int>(e => -e)
                               : Maybe<int>.Nothing;
    }
  }
}
