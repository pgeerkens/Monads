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
using System.Globalization;

namespace PGSolutions.Monads {
    using static CultureInfo;
    using static String;

    /// <summary>TODO</summary>
    public static class StructTuple {
        /// <summary>Expose type inference on the corresponding constructor for <see cref="StructTuple{TState,TValue}"/>.</summary>
        public  static StructTuple<TState,TValue> New<TState,TValue>(TState state, TValue value) =>
            new StructTuple<TState,TValue>(state, value);
    }

    /// <summary>Class delivered by an instance of the <see cref="State{TState,TValue}"/> monad.</summary>
    /// <typeparam name="TState">Type of the internal state.</typeparam>
    /// <typeparam name="TValue">Type of the delivered value.</typeparam>
    /// <remarks>
    ///  Minor observed performance penalty for small class vs small struct.
    /// </remarks>
    public struct StructTuple<TState, TValue> : IEquatable<StructTuple<TState, TValue>>, ISafeToString {
        /// <summary>Creates a new instance from a <typeparamref name="TState"/> and a <typeparamref name="TValue"/>.</summary>
        public StructTuple(TState state, TValue value) : this() { State = state;  Value = value; }

        /// <summary>Returns the current <typeparamref name="TState"/> value.</summary>
        public TState State { get; }
        /// <summary>Returns the current <typeparamref name="TValue"/> value.</summary>
        public TValue Value { get; }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        public override bool Equals(object obj) {
            var other = (obj as StructTuple<TState, TValue>?);
            return other.HasValue  &&  this.Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        public bool Equals(StructTuple<TState, TValue> other) =>
            Value.Equals(other.Value) && State.Equals(other.State);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(StructTuple<TState, TValue> lhs, StructTuple<TState, TValue> rhs) =>
            lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(StructTuple<TState, TValue> lhs, StructTuple<TState, TValue> rhs) =>
            !lhs.Equals(rhs);

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode() ^ State.GetHashCode();

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override string ToString() => Format(InvariantCulture,$"({State},{Value})",new object[0]);
        #endregion
    }
}
