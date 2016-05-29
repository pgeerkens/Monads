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
#define StateAsStruct
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace PGSolutions.Utilities.Monads {
    using static Contract;

    /// <summary>The State monad.</summary>
    /// <typeparam name="TState">Type of the internal state threaded by this instance.</typeparam>
    /// <typeparam name="TValue">Type of the calculated value exposed by this instance.</typeparam>
#if StateAsStruct
    public struct State<TState,TValue> : IEquatable<State<TState,TValue>> {
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate StatePayload<TState, TValue> StateTransform(TState s);
        /// <summary>TODO</summary>
        /// <param name="functor"></param>
        public State(StateTransform functor) : this() {
            functor.ContractedNotNull(nameof(functor));
            Ensures(_functor != null);

            _functor = functor;
        }

        /// <summary>TODO</summary>
        public StatePayload<TState,TValue> Invoke(TState state) => _functor(state);

        readonly StateTransform _functor;

        /// <summary>TODO</summary>
        /// <param name="functor"></param>
        [Pure]
        public static explicit operator State<TState,TValue>(StateTransform functor) =>
            new State<TState,TValue>(s=>functor(s));
        /// <summary>TODO</summary>
        /// <param name="functor"></param>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [Pure]
        public static State<TState,TValue> ToState(StateTransform functor) => (State<TState,TValue>)functor;

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Invariant(_functor != null);
        }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            var other = obj as State<TState, TValue>?;
            return other.HasValue && this.Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(State<TState,TValue> other) => _functor.Equals(other._functor);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => _functor.GetHashCode();

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Ensures(Result<string>() != null);
            return String.Format(CultureInfo.InvariantCulture, "({0})", _functor);
        }

        /// <summary>Tests reference-equality of the contained functor.</summary>
        [Pure]
        public static bool operator ==(State<TState,TValue> lhs, State<TState,TValue> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality of the contained functor</summary>
        [Pure]
        public static bool operator !=(State<TState,TValue> lhs, State<TState,TValue> rhs) => ! lhs.Equals(rhs);
        #endregion
    }
#else
    public delegate StatePayload<TState, TValue> State<TState, TValue>(TState s);
#endif

    /// <summary>TODO</summary>
    /// <typeparam name="TState">Type of the state to and from which this delegate selects</typeparam>
    /// <typeparam name="TValue">Type of the value which this delegate accepts</typeparam>
    public delegate State<TState,TValue>          Selector<TState,TValue>(TState s);
    /// <summary>TODO</summary>
    /// <typeparam name="TState">Type of the state to and from which this delegate selects</typeparam>
    /// <typeparam name="TValue">Type of the value which this delegate accepts</typeparam>
    /// <typeparam name="TResult">Type of the value which this delegate returns</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public delegate State<TState,TResult>         PayloadSelector<TState,TValue,TResult>(StatePayload<TState,TValue> p);

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    [Pure]
    public static class State {
        /// <summary>TODO</summary>
        public static State<TState,bool>          DoWhile<TState>( this
            State<TState,bool> @this
        ) {
            return new State<TState, bool>(s => {
                StatePayload<TState, bool> payload;
                do { payload = @this.Invoke(s); s = payload.State; } while (payload.Value);
                return payload;
            });
        }

        /// <summary>Generates an unending stream of successive StatePayload{TState,T} objects.</summary>
        public static IEnumerable<StatePayload<TState,TValue>>  Enumerate<TState,TValue>( this
            State<TState,TValue> @this,
            TState startState
        ) {
            Ensures(Result<IEnumerable<StatePayload<TState,TValue>>>() != null);

            var payload = new StatePayload<TState,TValue>(startState,default(TValue));
            while (true) yield return (payload = @this.Invoke(payload.State));
        }

        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static State<TState, TResult>      Select<TState, TValue, TResult>(this
            State<TState, TValue> @this,
            Func<TValue, TResult> projector
        ) {
            projector.ContractedNotNull(nameof(projector));

            return new State<TState,TResult>(
                s => new StatePayload<TState, TResult>(s, projector(@this.Invoke(s).Value))
            );
        }

        /// <summary>Implementation of Bind operator: (>>=): m a -> (a -> m b) -> m b.</summary>
        /// <remarks>
        /// Haskell operator: (>>=): m a -> (a -> m b) -> m b
        /// or:       mv >>= g = State (\st -> 
        ///                        let (y, st') = runState mv st
        ///                        in runState (g y) st')
        /// </remarks>
        public static State<TState,TResult>       SelectMany<TState,TValue,TResult> ( this
            State<TState,TValue> @this,
            PayloadSelector<TState, TValue, TResult> selector
        ) {
            selector.ContractedNotNull(nameof(selector));

            return new State<TState, TResult>(
                s => @this.Invoke(s).Bind(p => selector(p).Invoke(p.State))
            );
        }

        /// <summary>LINQ-compatible alias for ?join?.</summary>
        /// <remarks>
        ///   The usual monadic Join, but renamed to avoid collision with the eponymous auto-
        ///   generated LINQ functions.
        /// 
        ///     return  @this.Bind(                aval =>      // Always available from Bind()
        ///             selector(aval).Bind(       bval =>
        ///             new State<TState,TResult>( s => 
        ///             new State<TState,TResult>.StateTuple(projector(aval,bval),s) ) ) );
        /// </remarks>
        public static State<TState,TResult>       SelectMany<TState,TValue,T,TResult> ( this
            State<TState,TValue> @this,
            PayloadSelector<TState, TValue, T> selector,
            Func<TValue, T, TResult> projector
        ) {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));

            return new State<TState, TResult>(
                s =>
                  @this.Invoke(s).Bind(p => new StatePayload<TState,TResult>(s,
                    projector(p.Value, selector(p).Invoke(s).Value)) )
            );
        }

        /// <summary>TODO</summary>
        public static State<TState,TValue>        ToState<TState,TValue>( this TValue @this
        ) {
            return new State<TState, TValue>(
                 s => new StatePayload<TState,TValue>(s,@this)
            );
        }      

        /// <summary>Performs <see cref="selector"/> on the result from a Get.</summary>
        public static State<TState,Unit>          GetCompose<TState>(Selector<TState,Unit> selector) {
            selector.ContractedNotNull(nameof(selector));

            return new State<TState, Unit>(
                 s => selector(s).Invoke(s)
            );
        }

        /// <summary>Puts the transformed state and returns the original state.</summary>
        /// <remarks>Optimized implementation of Get.Compose(s => Put(transform(s))).</remarks>
        public static State<TState,TState>        Modify<TState>(Transform<TState> transform) {
            transform.ContractedNotNull(nameof(transform));

            return new State<TState, TState>(
                 s => new StatePayload<TState, TState>(transform(s), s)
            );
        }

        /// <summary>Puts the supplied state, resturning a Unit.</summary>
        public static State<TState,Unit>          Put<TState>(TState state) {
            state.ContractedNotNull(nameof(state));

            return (State<TState,Unit>) new StatePayload<TState,Unit>(state, Unit._);
        }

        /// <summary>Get's the current state as both State and Value.</summary>
        public static State<TState, TState> Get<TState>(TState state) {
            state.ContractedNotNull(nameof(state));

            return (State<TState,TState>) new StatePayload<TState, TState>(state, state);
        }
    }
}