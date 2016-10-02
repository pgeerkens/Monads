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
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>The State monad.</summary>
    /// <typeparam name="TState">Type of the internal state threaded by this instance.</typeparam>
    /// <typeparam name="TValue">Type of the calculated value exposed by this instance.</typeparam>
    public delegate StatePayload<TState,TValue>         LazyState<TState,TValue>(TState s);

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    [Pure]
    public static class LazyState {
        /// <summary>TODO</summary>
        /// <typeparam name="TState">Type of the state to and from which this delegate selects</typeparam>
        /// <typeparam name="TValue">Type of the value which this delegate accepts</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate LazyState<TState,TValue>        Selector<TState,TValue>(TState s);

        /// <summary>TODO</summary>
        public static LazyState<TState,bool>            DoWhile<TState>(this
            LazyState<TState,bool> @this
        ) {
            Ensures(Result<LazyState<TState,bool>>() != null);

            return s => {
                StatePayload<TState,bool> payload;
                do { payload = @this(s); s = payload.State; } while (payload.Value);
                return payload;
            };
        }

        /// <summary>Generates an unending stream of successive StructTuple{TState,T} objects.</summary>
        public static IEnumerable<StatePayload<TState,TValue>>  Enumerate<TState,TValue>(this
            LazyState<TState,TValue> @this,
            TState startState
        ) {
            Ensures(Result<IEnumerable<StatePayload<TState,TValue>>>() != null);

            var tuple = StatePayload.New(startState,default(TValue));
            while (true) yield return (tuple = @this(tuple.State));
        }

        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static LazyState<TState, TResult>        Select<TState, TValue, TResult>(this
            LazyState<TState, TValue> @this,
            Func<TValue, TResult> selector
        ) {
            selector.ContractedNotNull(nameof(selector));
            Ensures(Result<LazyState<TState, TResult>>() != null);

            return s => {
                var sourceResult = @this(s);
                return StatePayload.New(sourceResult.State,selector(sourceResult.Value));
            };
        }

        /// <summary>Implementation of Bind operator: (>>=): m a -> (a -> m b) -> m b.</summary>
        /// <remarks>
        /// Haskell operator: (>>=): m a -> (a -> m b) -> m b
        /// or:     mv >>= g = State (\st -> 
        ///                      let (y, st') = runState mv st
        ///                      in runState (g y) st')
        /// or:     return s => {
        ///             var sourceResult = @this(s);
        ///             return selector(sourceResult.Value)(sourceResult.State);
        ///         };
        /// </remarks>
        public static LazyState<TState,TResult>         SelectMany<TState,TValue,TResult> (this
            LazyState<TState,TValue> @this,
            Func<TValue,LazyState<TState,TResult>> selector
        ) {
            selector.ContractedNotNull(nameof(selector));
            Ensures(Result<LazyState<TState, TResult>>() != null);

            return s => {
                var sourceResult = @this(s);
                return selector(sourceResult.Value)(sourceResult.State);
            };
        }

        /// <summary>LINQ-compatible alias for join.</summary>
        /// <remarks>
        ///     return  @this.SelectMany(          aval =>      // Always available from Bind()
        ///             selector(aval).SelectMany( bval =>
        ///             new State{TState,TResult}( s    => 
        ///             new StatePayload{TState,TResult}(s,projector(aval,bval)) ) ) );
        /// </remarks>
        public static LazyState<TState,TResult>         SelectMany<TState,TValue,T,TResult> (this
            LazyState<TState,TValue> @this,
            Func<TValue, LazyState<TState,T>> selector,
            Func<TValue, T, TResult> projector
        ) {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));
            Ensures(Result<LazyState<TState, TResult>>() != null);

            return s => {
                var sourceResult = @this(s);
                var selectorResult = selector(sourceResult.Value)(sourceResult.State);
                return StatePayload.New(selectorResult.State,
                            projector(sourceResult.Value, selectorResult.Value)
                    );
            };
        }

        /// <summary>TODO</summary>
        public static LazyState<TState, TValue>         ToLazyState<TState, TValue>(this TValue @this
        ) {
            Ensures(Result<LazyState<TState, TValue>>() != null);

            return s => StatePayload.New(s,@this);
        }

        /// <summary>Get's the current state as both State and Value.</summary>
        public static LazyState<TState, TState>         Get<TState>() {
            Ensures(Result<LazyState<TState,TState>>() != null);

            return state => StatePayload.New(state, state);
        }

        /// <summary>Performs <param name="selector"/> on the result from a Get.</summary>
        public static LazyState<TState,Unit>            GetCompose<TState>(Selector<TState,Unit> selector) {
            selector.ContractedNotNull(nameof(selector));
            Ensures(Result<LazyState<TState,Unit>>() != null);

            return s => selector(s)(s);
        }

        /// <summary>Puts the supplied state, resturning a Unit.</summary>
        public static LazyState<TState,Unit>            Put<TState>(TState state) {
            state.ContractedNotNull(nameof(state));
            Ensures(Result<LazyState<TState,Unit>>() != null);

            return s => StatePayload.New(state, Unit._);
        }
    }
}
