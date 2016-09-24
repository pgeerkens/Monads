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

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>Class factory for StatePayload{TState,TValue}, with conveninece methods that perform constructor type inference.</summary>
    public static class StatePayload {
        /// <summary>Expose type inference on the corresponding constructor for StatePayload{TState,TValue}.</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static StatePayload<TState,TValue> New<TState,TValue>(Func<StructTuple<TState, TValue>> valueFactory) {
            valueFactory.ContractedNotNull(nameof(valueFactory));
            return new StatePayload<TState,TValue>(valueFactory);
        }
        /// <summary>Expose type inference on the corresponding constructor for StatePayload{TState,TValue}.</summary>
        public static StatePayload<TState,TValue> New<TState,TValue>(StructTuple<TState, TValue> tuple) {
            return new StatePayload<TState,TValue>(tuple);
        }
        /// <summary>Expose type inference on the corresponding constructor for StatePayload{TState,TValue}.</summary>
        public  static StatePayload<TState,TValue> New<TState,TValue>(TState state, TValue value) {
            state.ContractedNotNull(nameof(state));
            value.ContractedNotNull(nameof(value));
            return new StatePayload<TState,TValue>(state, value);
        }
    }

    /// <summary>TODO</summary>
    public struct StatePayload<TState,TValue> {
        /// <summary>Return a new StatePayload{TState,TValue} instance.</summary>
        internal StatePayload(Func<StructTuple<TState, TValue>> valueFactory) {
            valueFactory.ContractedNotNull(nameof(valueFactory));

            _base = new Lazy<StructTuple<TState,TValue>>(valueFactory);
        }
        /// <summary>Return a new StatePayload{TState,TValue} instance.</summary>
        internal StatePayload(StructTuple<TState, TValue> tuple) : this(() => tuple) { ; }
        /// <summary>Return a new StatePayload{TState,TValue} instance.</summary>
        internal StatePayload(TState state, TValue value) : this(StructTuple.New(state,value)) {
            state.ContractedNotNull(nameof(state));
            value.ContractedNotNull(nameof(value));
        }

        private readonly Lazy<StructTuple<TState,TValue>> _base;

        /// <inheritdoc/>
        public TState State { get { Ensures(Result<TState>() != null);  return _base.Value.State; } }
        /// <inheritdoc/>
        public TValue Value { get { Ensures(Result<TValue>() != null);  return _base.Value.Value; } }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            var other = (obj as StatePayload<TState, TValue>?);
            return other.HasValue  &&  this.Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(StatePayload<TState, TValue> other) =>
            Value.Equals(other.Value) && State.Equals(other.State);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() { unchecked { return Value.GetHashCode() ^ State.GetHashCode(); } }

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Ensures(Result<string>() != null);
            return  "({0},{1})".FormatMe(State,Value);
        }

        /// <summary>Tests value-equality.</summary>
        [Pure]
        public static bool operator ==(StatePayload<TState, TValue> lhs, StatePayload<TState, TValue> rhs) =>
            lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        [Pure]
        public static bool operator !=(StatePayload<TState, TValue> lhs, StatePayload<TState, TValue> rhs) =>
            !lhs.Equals(rhs);
        #endregion
    }
}
