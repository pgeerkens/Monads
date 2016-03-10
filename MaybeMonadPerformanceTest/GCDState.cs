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
//#define GCDStateAsClass    // performance penalty of 0% to 40% for class compared to struct

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
#if GCDStateAsClass
  internal class GCDState : IEquatable<GCDState> {
#else
  public struct GCDState : IEquatable<GCDState> {
#endif
    static readonly CultureInfo _culture = CultureInfo.CurrentUICulture;

    public GCDState(int a, int b) { _a = a; _b = b; }

    public int A { get { return _a; } } private readonly int _a;
    public int B { get { return _b; } } private readonly int _b;

    /// <inheritdoc/>
    public override string ToString() { 
      return String.Format(_culture,"({0,14:N0}, {1,14:N0})",A,B);
    }

    #region Value Equality with IEquatable<T>.
    /// <inheritdoc/>
    [Pure]
    public override bool Equals(object obj) {
#if GCDStateAsClass
      var other = obj as GCDState;
      return other != null  &&  other.Equals(obj);
#else
      var other = obj as GCDState?;
      return other.HasValue && other.Equals(obj);
#endif
    }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]
    public bool Equals(GCDState other) { return this.A == other.A && this.B == other.B; }

    /// <inheritdoc/>
    [Pure]
    public override int GetHashCode() { unchecked { return A ^ B; } }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]
    public static bool operator ==(GCDState lhs, GCDState rhs) { return lhs.Equals(rhs); }

    /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
    [Pure]
    public static bool operator !=(GCDState lhs, GCDState rhs) { return !lhs.Equals(rhs); }
    #endregion
  }

  public struct ResultValue : IEquatable<ResultValue> {
    static readonly CultureInfo _culture = CultureInfo.CurrentUICulture;

    public ResultValue(int gcd) {
        _gcd    = gcd;
    //    _title  = title;
    }

    public int      Gcd     { get {return _gcd;  } } private readonly int      _gcd;
  //  public string   Title   { get {return _title;} } private readonly string   _title;

    /// <inheritdoc/>
    public override string ToString() { 
        return String.Format(_culture,"    GCD = {0}",Gcd); //, Title);
    }

    #region Value Equality with IEquatable<T>.
    /// <inheritdoc/>
    [Pure]
    public override bool Equals(object obj) {
      var other = obj as ResultValue?;
      return other.HasValue && other.Equals(obj);
    }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]
    public bool Equals(ResultValue other) { return this.Gcd == other.Gcd; }

    /// <inheritdoc/>
    [Pure]
    public override int GetHashCode() { unchecked { return Gcd.GetHashCode(); } }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]
    public static bool operator ==(ResultValue lhs, ResultValue rhs) { return lhs.Equals(rhs); }

    /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
    [Pure]
    public static bool operator !=(ResultValue lhs, ResultValue rhs) { return !lhs.Equals(rhs); }
    #endregion
  }
}
