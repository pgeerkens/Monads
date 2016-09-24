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

    /// <summary>TODO</summary>
    public static class StructTuple {
        /// <summary>Expose type inference on the corresponding constructor for StatePayload{TState,TValue}.</summary>
        public  static StructTuple<TState,TValue> New<TState,TValue>(TState state, TValue value) {
            state.ContractedNotNull(nameof(state));
            value.ContractedNotNull(nameof(value));
            return new StructTuple<TState,TValue>(state, value);
        }
    }

    /// <summary>Class delivered by an instance of the <see cref="State{TState,TValue}"/> monad.</summary>
    /// <typeparam name="TState">Type of the internal state.</typeparam>
    /// <typeparam name="TValue">Type of the delivered value.</typeparam>
    /// <remarks>
    ///  Minor observed performance penalty for small class vs small struct.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
        Justification = "This nested type shares the Generic Type parameters of its parent, and is structurally associated with it.")]
    public struct StructTuple<TState, TValue> : IEquatable<StructTuple<TState, TValue>> {
        /// <summary>TODO</summary>
        public StructTuple(TState state, TValue value) : this() {
            state.ContractedNotNull(nameof(state));
            value.ContractedNotNull(nameof(value));
            Ensures(State != null);
            Ensures(Value != null);

            State = state;  Value = value;
            Assert(State != null); Assert(Value != null);   // required by limitation in static checker
        }

        /// <summary>TODO</summary>
        public TState State { get; }
        /// <summary>TODO</summary>
        public TValue Value { get; }

        ///// <summary>Implementation of <i>Bind</i> for an Identity monad.</summary>
        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        //[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        //[Pure]
        //public StructTuple<TState,TResult> Bind<TResult>(
        //    Func<StructTuple<TState,TValue>, StructTuple<TState,TResult>> selector
        //) {
        //    selector.ContractedNotNull(nameof(selector));
        // //   Ensures(Result<StructTuple<TState,TResult>>() != null);

        //    return selector(this);
        //}

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]private void ObjectInvariant() {
            Invariant( State != null );
            Invariant( Value != null );
        }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            var other = (obj as StructTuple<TState, TValue>?);
            return other.HasValue  &&  this.Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(StructTuple<TState, TValue> other) =>
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
        public static bool operator ==(StructTuple<TState, TValue> lhs, StructTuple<TState, TValue> rhs) =>
            lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        [Pure]
        public static bool operator !=(StructTuple<TState, TValue> lhs, StructTuple<TState, TValue> rhs) =>
            !lhs.Equals(rhs);
        #endregion
    }
}
