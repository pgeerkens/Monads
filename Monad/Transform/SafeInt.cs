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
using System.Diagnostics.Contracts;
using System.Globalization;

namespace PGSolutions.Monads {
    using static CultureInfo;
    using static Contract;

    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    public interface ISafeEquatable<T> {
        /// <summary>TODO</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool? Equals(SafeInt other);
    }
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    public interface ISafeComparable<T> {
        /// <summary>TODO</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        int? CompareTo(SafeInt other);
    }

    /// <summary>TODO</summary>
    public struct SafeInt : ISafeEquatable<SafeInt>, IEquatable<SafeInt>, ISafeComparable<SafeInt>, ISafeToString {
        ///<summary>Create a new Maybe{T}.</summary>
        private SafeInt(int? value) : this() {
            _value    = value;
        }
     
        /// <summary>TODO</summary>
        public int? Value => _value;   private readonly int? _value;

        /// <summary>TODO</summary>
        public static implicit operator SafeInt(int? value) => new SafeInt(value);

        /// <summary>TODO</summary>
        /// <param name="lhs"></param>
        /// <param name="defaultValue"></param>
        public static int operator | (SafeInt lhs, int defaultValue) =>
             lhs._value.HasValue ? lhs._value.Value : defaultValue;
        /// <summary><see cref="BitwiseOr"/></summary>
        public static int BitwiseOr(SafeInt lhs, int defaultValue) => lhs | defaultValue;

        /// <summary>Returns a SafeInt with the sum, or null if the operation fails.</summary>
        public static SafeInt operator + (SafeInt addend1, SafeInt addend2) =>
            addend1._value.SelectMany(l =>
            addend2._value.SelectMany(r => l.SafeAddition(r), Functions.Second), Functions.Second);
        /// <summary><see cref="Add"/></summary>
        public static SafeInt Add(SafeInt lhs, SafeInt rhs) => lhs + rhs;

        /// <summary>Returns a SafeInt with the difference, or null if the operation fails.</summary>
        public static SafeInt operator - (SafeInt lhs, SafeInt rhs) =>
            lhs._value.SelectMany(l =>
            rhs._value.SelectMany(r => l.SafeSubtract(r), Functions.Second), Functions.Second);

        /// <summary><see cref="Subtract"/></summary>
        public static SafeInt Subtract(SafeInt minuend, SafeInt subtrahend) =>
            minuend - subtrahend;

        /// <summary>Returns a SafeInt with the quotient, or null if the operation fails.</summary>
        public static SafeInt operator / (SafeInt dividend, SafeInt divisor) =>
            dividend._value.SelectMany(lhs =>
            divisor ._value.SelectMany(rhs => lhs.SafeDivide(rhs), Functions.Second), Functions.Second);

        /// <summary><see cref="Divide"/></summary>
        public static SafeInt Divide(SafeInt lhs, SafeInt rhs) => lhs / rhs;

        /// <inheritdoc/>
        public int? CompareTo(SafeInt other) =>
            //from lhs in _value from rhs in other._value select lhs.CompareTo(rhs);
            _value.HasValue && other._value.HasValue ? _value.Value.CompareTo(other._value.Value)
                                                     : default(int?);

        /// <inheritdoc/>
        bool? ISafeEquatable<SafeInt>.Equals(SafeInt other) => this.AreNonNullEqual(other);

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        [Pure]public override bool Equals(object obj) => (obj as SafeInt?)?.Equals(this) ?? false;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]public bool Equals(SafeInt other) => _value.Equals(other._value);

        /// <summary>Tests value-equality, returning false if either value doesn't exist.</summary>
        [Pure]public static bool operator ==(SafeInt lhs, SafeInt rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning false if either value doesn't exist..</summary>
        [Pure]public static bool operator !=(SafeInt lhs, SafeInt rhs) => !lhs.Equals(rhs);

        ///<summary>Retrieves the hash code of the object returned by the <see cref="_value"/> property.</summary>
        [Pure]public override int GetHashCode() => (_value == null) ? 0 : _value.GetHashCode();

        /// <inheritdoc/>
        [Pure]
        public override string ToString() => _value?.ToString(InvariantCulture) ?? "";
        #endregion
    }

    /// <summary>TODO</summary>
    /// <remarks>
    /// courtesy Ivan Stoev: http://w3foverflow.com/question/integer-overflow-detection-c-for-add/
    /// See also <href a="https://www.fefe.de/intof.html">Catching Integer Overflows in C</href>
    /// </remarks>
    [Pure]
    public static class SafeIntExtensions {
        /// <summary>Returns a Nullable{int} with the sum, or null if the operation fails.</summary>
        public static int? SafeAddition(this int addend1, int addend2) {
            unchecked {
                var c = addend1 + addend2;
                return ((addend1 ^ addend2) >= 0) & ((addend1 ^ c) < 0) ? default(int?)
                                                                        : c;
            }
        }

        /// <summary>Returns a Nullable{int} with the difference, or null if the operation fails.</summary>
        public static int? SafeSubtract(this int minuend, int subtrahend) =>
            subtrahend != int.MinValue ? minuend.SafeAddition(-subtrahend)
                        : minuend >= 0 ? default(int?)
                                       : int.MaxValue.SafeAddition(minuend).Select<int,int>(e => e+1);
                                       //: from e in int.MaxValue.SafeAddition(minuend)
                                       //  select e+1;

        /// <summary>Returns a Nullable{int} with the quotient, or null if the operation fails.</summary>
        public static int? SafeDivide(this int dividend, int divisor) =>
            divisor == 0 || (dividend == int.MinValue && divisor == -1)
                          ? default(int?)
                          : dividend / divisor;

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        public static bool? AreNonNullEqual(this SafeInt lhs, SafeInt rhs) =>
            lhs.Value.HasValue && rhs.Value.HasValue ? lhs.Value.Equals(rhs.Value)
                                                     : null as bool?;

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        public static bool? AreNonNullUnequal(this SafeInt lhs, SafeInt rhs) =>
           lhs. Value.HasValue && rhs.Value.HasValue ? ! lhs.Value.Equals(rhs.Value)
                                                     : null as bool?;
    }
}
