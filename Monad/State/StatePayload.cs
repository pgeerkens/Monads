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
using System.Diagnostics.Contracts;
using System.Globalization;

namespace PGSolutions.Utilities.Monads {
    using static Contract;

    /// <summary>Class delivered by an instance of the <see cref="State{TState,TValue}"/> monad.</summary>
    /// <typeparam name="TState">Type of the internal state.</typeparam>
    /// <typeparam name="TValue">Type of the delivered value.</typeparam>
    /// <remarks>
    ///  Minor observed performance penalty for small class vs small struct.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
        Justification = "This nested type shares the Generic Type parameters of its parent, and is structurally associated with it.")]
    public struct StatePayload<TState, TValue> : IEquatable<StatePayload<TState, TValue>> {
        public StatePayload(TState state, TValue value) : this() {
            state.ContractedNotNull("state");
            value.ContractedNotNull("value");
            Ensures(_state != null);
            Ensures(_value != null);

            _state = state;  _value = value;
        }

        public TState State {
            get {
                Ensures((_state != null) == (State != null));
                return _state;
            } } readonly TState _state;
        public TValue Value {
            get {
                Ensures((_value != null) == (Value != null));
                return _value;
            } } readonly TValue _value;

        [Pure]
        public static explicit operator State<TState,TValue>(StatePayload<TState, TValue> payload) {
            Ensures(Result<State<TState,TValue>>() != null);

            return s => payload;
        }
        [Pure]
        public static State<TState, TValue> ToState(StatePayload<TState, TValue> payload) {
            Ensures(Result<State<TState,TValue>>() != null);

            return s => payload;
        }

        /// <summary>Implementation of <i>Bind</i> for an Identity monad.</summary>
        [Pure]
        public StatePayload<TState,TResult> Bind<TResult>(
            Func<StatePayload<TState,TValue>, StatePayload<TState,TResult>> selector
        ) {
            selector.ContractedNotNull("selector");

            return selector(this);
        }

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]private void ObjectInvariant() {
            Invariant(_state != null);  Invariant((_state != null) == (State != null));
            Invariant(_value != null);  Invariant((_value != null) == (Value!=null));
            Invariant( State != null );
            Invariant( Value != null );
        }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            var other = (obj as StatePayload<TState, TValue>?).ToMaybe();
            var @this = this;
            return other.SelectMany<bool>(o => o.Equals(@this)) | false;
            //return other != null  &&  this.Equals(other);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(StatePayload<TState, TValue> other) {
            return this.Value.Equals(other.Value) && this.State.Equals(other.State);
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() { unchecked { return Value.GetHashCode() ^ State.GetHashCode(); } }

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Ensures(Result<string>() != null);
            return String.Format(CultureInfo.InvariantCulture, "({0},{1})",Value, State);
        }

        /// <summary>Tests value-equality.</summary>
        [Pure]
        public static bool operator ==(StatePayload<TState, TValue> lhs, StatePayload<TState, TValue> rhs) {
          return lhs.Equals(rhs);
        }

        /// <summary>Tests value-inequality.</summary>
        [Pure]
        public static bool operator !=(StatePayload<TState, TValue> lhs, StatePayload<TState, TValue> rhs) {
          return !lhs.Equals(rhs);
        }
        #endregion
    }
}
