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

namespace PGSolutions.Monads {
    /// <summary>The State monad.</summary>
    /// <typeparam name="TState">Type of the internal state threaded by this instance.</typeparam>
    /// <typeparam name="TValue">Type of the calculated value exposed by this instance.</typeparam>
    public delegate ValueTuple<TState,TValue>      State<TState,TValue>(TState s);

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    public static class State {
        /// <summary>TODO</summary>
        /// <typeparam name="TState">Type of the state to and from which this delegate selects</typeparam>
        /// <typeparam name="TValue">Type of the value which this delegate accepts</typeparam>  
        public delegate State<TState,TValue>    Selector<TState,TValue>(TState s);

        /// <summary>Expose type inference on the corresponding constructor for <see cref="ValueTuple{TState,TValue}"/>.</summary>
        public static ValueTuple<TState, TValue> NewPayload<TState, TValue>(TState state, TValue value) =>
            new ValueTuple<TState, TValue>(state, value);

        /// <summary>TODO</summary>
        public static State<TState,bool>        DoWhile<TState>(this
            State<TState,bool> @this
        ) => s => {
                ValueTuple<TState,bool> payload;
                do { payload = @this(s); s = payload.Item1; } while (payload.Item2);
                return payload;
            };

        /// <summary>Generates an unending stream of successive <see cref="ValueTuple{TState,T}"/> objects.</summary>
        public static IEnumerable<ValueTuple<TState,TValue>>  Enumerate<TState,TValue>(this
            State<TState,TValue> @this,
            TState startState
        ) {
            var tuple = NewPayload(startState,default(TValue));
            while (true) yield return (tuple = @this(tuple.Item1));
        }

        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static State<TState, TResult>    Select<TState, TValue, TResult>(this
            State<TState, TValue> @this,
            Func<TValue, TResult> selector
        ) {
            selector.ContractedNotNull(nameof(selector));
            return @this.SelectMany<TState,TValue,TResult>(x => s => NewPayload(s,selector(x)));
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
        public static State<TState,TResult>     SelectMany<TState,TValue,TResult> (this
            State<TState,TValue> @this,
            Func<TValue,State<TState,TResult>> selector
        ) {
            selector.ContractedNotNull(nameof(selector));
            return @this.SelectMany(selector, Functions.Second);
        }

        /// <summary>LINQ-compatible alias for join.</summary>
        /// <remarks>
        ///     return  @this.SelectMany(          aval =>      // Always available from Bind()
        ///             selector(aval).SelectMany( bval =>
        ///             new State{TState,TResult}( s    => 
        ///             new StatePayload{TState,TResult}(s,projector(aval,bval)) ) ) );
        /// </remarks>
        public static State<TState,TResult>     SelectMany<TState,TValue,T,TResult> (this
            State<TState,TValue> @this,
            Func<TValue, State<TState,T>> selector,
            Func<TValue, T, TResult> projector
        ) {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));

            return s => {
                var sourceResult = @this(s);
                var selectorResult = selector(sourceResult.Item2)(sourceResult.Item1);
                return NewPayload(selectorResult.Item1,
                            projector(sourceResult.Item2, selectorResult.Item2)
                    );
            };
        }

        /// <summary>TODO</summary>
        public static State<TState, TValue>     ToState<TState, TValue>(this TValue @this
        ) => s => NewPayload(s,@this);

        /// <summary>Get's the current state as both State and Value.</summary>
        public static State<TState, TState>     Get<TState>() => state => NewPayload(state, state);

        /// <summary>Performs <param name="selector"/> on the result from a Get.</summary>
        public static State<TState,Unit>        GetCompose<TState>(Selector<TState,Unit> selector) {
            selector.ContractedNotNull(nameof(selector));
            return s => selector(s)(s);
        }

        /// <summary>Puts the supplied state, resturning a Unit.</summary>
        public static State<TState,Unit>        Put<TState>(TState state) {
            state.ContractedNotNull(nameof(state));

            return s => NewPayload(state, Unit._);
        }
    }


    /// <summary>TODO</summary>
    public static class MaybeState {
        /// <summary>TODO</summary>
        public static X<State<TState, bool>>    DoWhile<TState>(this
            X<State<TState, bool>> @this
        ) => from me in @this select me.DoWhile();
    }
}