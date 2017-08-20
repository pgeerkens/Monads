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

namespace PGSolutions.Monads {
    using static FormattableString;

    /// <summary>Class factory for <see cref="LazyPayload{TState,TValue}"/>, with conveninece methods that perform constructor type inference.</summary>
    public static class LazyPayload {
        /// <summary>Expose type inference on the corresponding constructor for <see cref="LazyPayload{TState,TValue}"/>.</summary>
        public static LazyPayload<TState,TValue> New<TState,TValue>(TState state, TValue value) =>
            new LazyPayload<TState,TValue>(state, value);
    }

    /// <summary>TODO</summary>
    /// <typeparam name="TState">Type of the internal state.</typeparam>
    /// <typeparam name="TValue">Type of the delivered value.</typeparam>
    public struct LazyPayload<TState,TValue> {
        /// <summary>Return a new object instance.</summary>
        internal LazyPayload(TState state, TValue value) : this(new ValueTuple<TState,TValue>(state,value)) { ; }
        /// <summary>Return a new object instance.</summary>
        private LazyPayload(ValueTuple<TState, TValue> tuple) : this(() => tuple) { ; }
        /// <summary>Return a new object instance.</summary>
        private LazyPayload(Func<ValueTuple<TState, TValue>> valueFactory) {
            _base = new Lazy<ValueTuple<TState,TValue>>(valueFactory);
        }

        private readonly Lazy<ValueTuple<TState,TValue>> _base;

        /// <summary>Returns the calculated current <typeparamref name="TState"/> value.</summary>
        public TState State => _base.Value.Item1;
        /// <summary>Returns the calculated current <typeparamref name="TValue"/> value.</summary>
        public TValue Value => _base.Value.Item2;

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        public override bool Equals(object obj) => 
                (obj as LazyPayload<TState,TValue>?)?.Equals(this) ?? false;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        public bool Equals(LazyPayload<TState, TValue> other) => Value.Equals(other.Value) && State.Equals(other.State);

        /// <summary>Tests value-equality.</summary>
        public static bool operator ==(LazyPayload<TState,TValue> lhs, LazyPayload<TState,TValue> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality.</summary>
        public static bool operator !=(LazyPayload<TState,TValue> lhs, LazyPayload<TState,TValue> rhs) => !lhs.Equals(rhs);

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode() ^ State.GetHashCode();
        /// <inheritdoc/>
        public override string ToString() => Invariant($"({State},{Value})");
        #endregion
    }
}
