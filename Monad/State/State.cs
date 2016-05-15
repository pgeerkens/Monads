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
using System.Linq;

namespace PGSolutions.Utilities.Monads {

    /// <summary>The State monad.</summary>
    /// <typeparam name="TValue">Type of the calculated value exposed by this instance.</typeparam>
    /// <typeparam name="TState">Type of the internal state threaded by this instance.</typeparam>
    public delegate StatePayload<TState,TValue>   State<TState, TValue>(TState s);

    /// <summary>TODO</summary>
    /// <typeparam name="TState">Type of the state which this delegate selects</typeparam>
    public delegate State<TState,TValue>          Selector<TState,TValue>(TState s);

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    [Pure]
    public static class State {
        /// <summary>TODO</summary>
        public static State<TState,bool>          DoWhile<TState>( this
            State<TState,bool> @this
        ) {
            @this.ContractedNotNull("this");
            Contract.Ensures(Contract.Result<State<TState,bool>>() != null);

            return s => {
                StatePayload<TState,bool> payload;
                do { payload = @this(s); s = payload.State; } while (payload.Value);
                return payload;
            };
        }

        /// <summary>Generates an unending stream of successive StatePayload&amp;TState,T> objects.</summary>
        public static IEnumerable<StatePayload<TState,TValue>>  Enumerate<TState,TValue>( this
            State<TState,TValue> @this,
            TState startState
        ) {
            @this.ContractedNotNull("this");
            Contract.Ensures(Contract.Result<IEnumerable<StatePayload<TState,TValue>>>() != null);

            var payload = new StatePayload<TState,TValue>(startState,default(TValue));
            while (true) yield return (payload = @this(payload.State));
        }

        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static State<TState, TResult>      Select<TState, TValue, TResult>(this
            State<TState, TValue> @this,
            Func<TValue, TResult> projector
        ) {
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<State<TState, TResult>>() != null);

            return s => new StatePayload<TState, TResult>(s, projector(@this(s).Value));
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
          Func<StatePayload<TState,TValue>, State<TState,TResult>> selector
        ) {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

            return s => @this(s).Bind(p => selector(p)(p.State));
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
            Func<StatePayload<TState,TValue>, State<TState,T>> selector,
            Func<TValue, T, TResult> projector
        ) {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

            return s =>
              @this(s).Bind(p => new StatePayload<TState,TResult>(s,
                projector(p.Value, selector(p)(s).Value)) );
        }

        /// <summary>TODO</summary>
        public static State<TState,TValue>        ToState<TState,TValue>( this TValue @this
        ) {
            Contract.Ensures(Contract.Result<State<TState,TValue>>() != null);
            return s => new StatePayload<TState,TValue>(s,@this);
        }      

        /// <summary>Performs <see cref="selector"/> on the result from a Get.</summary>
        public static State<TState,Unit>          GetCompose<TState>(Selector<TState,Unit> selector) {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<State<TState,Unit>>() != null);

            return s => selector(s)(s);;
        }

        /// <summary>Puts the transformed state and returns the original state.</summary>
        /// <remarks>Optimized implementation of Get.Compose(s => Put(transform(s))).</remarks>
        public static State<TState,TState>        Modify<TState>(Transform<TState> transform) {
            transform.ContractedNotNull("transform");
            Contract.Ensures(Contract.Result<State<TState,TState>>() != null);

#if  true
            return s => new StatePayload<TState,TState>(transform(s),s);

#else
            return new _modify(transform).Run;
        }

        private class _modify {
            public _modify(Transform<TState> transform) {
                transform.ContractedNotNull("transform");
                _transform = transform;
            }
            private readonly Transform<TState> _transform;

            public StatePayload<TState,TState> Run(TState s) {
                return new StatePayload<TState,TState>(_transform(s),s);
            }

            /// <summary>The invariants enforced by this struct type.</summary>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            [ContractInvariantMethod]
            [Pure]private void ObjectInvariant() {
              Contract.Invariant( _transform != null );
            }
#endif
        }

        /// <summary>Puts the supplied state, resturning a Unit.</summary>
        public static State<TState,Unit>          Put<TState>(TState state) {
            state.ContractedNotNull("state");
            Contract.Ensures(Contract.Result< State<TState,Unit> >() != null);

            return (State<TState,Unit>) new StatePayload<TState,Unit>(state, Unit._);
        }

        /// <summary>Get's the current state as both State and Value.</summary>
        public static State<TState, TState> Get<TState>(TState state) {
            state.ContractedNotNull("state");
            Contract.Ensures(Contract.Result<State<TState, TState>>() != null);

            return (State<TState,TState>) new StatePayload<TState, TState>(state, state);
        }
    }
}