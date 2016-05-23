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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if false
namespace PGSolutions.Utilities.Monads {
    public struct StateMonad<TState, TValue> : IEquatable<StateMonad<TState, TValue>> {
        public StateMonad(TState state, TValue value) : this() {
            state.ContractedNotNull("state");
            value.ContractedNotNull("value");
            Contract.Ensures(_state != null);
            Contract.Ensures(_value != null);

            _state = state; _value = value;
        }

        [Pure]
        public TState State {
            get {
                Contract.Ensures((_state != null) == (State != null));
                return _state;
            }
        }
        readonly TState _state;
        [Pure]
        public TValue Value {
            get {
                Contract.Ensures((_value != null) == (Value != null));
                return _value;
            }
        }
        readonly TValue _value;

        [Pure]
        public static explicit operator StateMonad<TState, TValue>(Tuple<TState, TValue> tuple) {
            Contract.Ensures(Contract.Result<StateMonad<TState, TValue>>() != null);

            return new StateMonad<TState, TValue>(tuple.Item1, tuple.Item2);
        }
        [Pure]
        public static StateMonad<TState, TValue> ToState(Tuple<TState, TValue> tuple) {
            Contract.Ensures(Contract.Result<StateMonad<TState, TValue>>() != null);

            return new StateMonad<TState, TValue>(tuple.Item1,tuple.Item2);
        }

        /// <summary>Implementation of <i>Bind</i> for an Identity monad.</summary>
        [Pure]
        public StateMonad<TState, TResult> Bind<TResult>(
            Func<StateMonad<TState, TValue>, StateMonad<TState, TResult>> selector
        ) {
            selector.ContractedNotNull("selector");

            return selector(this);
        }

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Contract.Invariant(_state != null); Contract.Invariant((_state != null) == (State != null));
            Contract.Invariant(_value != null); Contract.Invariant((_value != null) == (Value!=null));
            Contract.Invariant(State != null);
            Contract.Invariant(Value != null);
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
        public bool Equals(StateMonad<TState, TValue> other) {
#if PayloadAsClass
            other.ContractedNotNull("other");
#endif
            return this.Value.Equals(other.Value) && this.State.Equals(other.State);
        }

        [Pure]
        bool IEquatable<StateMonad<TState, TValue>>.Equals(StateMonad<TState, TValue> other) {
            return this.Equals(other);
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() { unchecked { return Value.GetHashCode() ^ State.GetHashCode(); } }

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return String.Format(CultureInfo.InvariantCulture, "({0},{1})", Value, State);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator ==(StateMonad<TState, TValue> lhs, StateMonad<TState, TValue> rhs) {
            return lhs.Equals(rhs);
        }

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator !=(StateMonad<TState, TValue> lhs, StateMonad<TState, TValue> rhs) {
            return !lhs.Equals(rhs);
        }
#endregion
    }
}
#endif
