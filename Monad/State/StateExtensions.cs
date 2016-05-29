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
using System.Diagnostics.Contracts;

namespace PGSolutions.Utilities.Monads {
    using static Contract;

    /// <summary>TODO</summary>
    /// <typeparam name="TState">Type of the state which this delegate transforms.</typeparam>
    public delegate TState Transform<TState>(TState s);

    /// <summary>Extension methods for <see cref="StateTuple<TState,TValue>"/>.</summary>
    [Pure]
    public static class StateExtensions {

        /// <summary>Implementation of <b>compose</b>: (>=>): f >=> g = \x -> (f x >>= g). </summary>
        /// <remarks> Optimized implementation of:
        ///         return this.Bind(t => follower(t.Item2));
        /// </remarks>
        public static State<TState,TResult>   Compose<TState,TValue,TResult> ( this
            State<TState, TValue> @this,
            Func<TState, State<TState,TResult>> follower
        ) {
            follower.ContractedNotNull(nameof(follower));

            return new State<TState,TResult>( s => follower(s).Invoke(@this.Invoke(s).State) );
        }

        /// <summary>Implementation of <b>then</b>: (>>):  mv1 >> mv2  =  mv1 >>= (\_ -> mv2)</summary>
        /// <remarks> Optimized implementation of:
        ///         return @this.Bind(t => follower);
        /// or
        ///         return @this.Then(s => follower);
        /// </remarks>
        public static State<TState,TResult>   Then<TState,TValue,TResult>( this
            State<TState, TValue> @this,
            State<TState, TResult> follower
        ) {
            return new State<TState,TResult>( s => follower.Invoke(@this.Invoke(s).State) );
        }

        /// <summary>TODO</summary>
        public static State<TState, Unit> DoWhile<TState>(this
            Transform<TState> @this, 
            Predicate<TState> predicate
        ) {
            @this.ContractedNotNull(nameof(@this));
            //Ensures(Result<State<TState, Unit>>() != null);

            return new State<TState,Unit>(
                    s => {
                    while (predicate(s)) { s = @this.Invoke(s); }
                    return new StatePayload<TState, Unit>(s, Unit._);
                }
            );
        }

        /// <summary>Generates an unending stream of successive TState objects.</summary>
        public static IEnumerable<TState> Enumerate<TState>(this
            Transform<TState> @this,
            TState startState
        ) {
            @this.ContractedNotNull(nameof(@this));
            startState.ContractedNotNull(nameof(startState));
            Ensures(Result<IEnumerable<TState>>() != null);

            while (true) yield return (startState = @this(startState));
        }

        /// <summary>Puts the transformed state and returns the original.</summary>
        public static State<TState, TState> Modify<TState>(this
            Transform<TState> @this
        ) {
            @this.ContractedNotNull(nameof(@this));
            //Ensures(Result<State<TState, TState>>() != null);

            return State.Modify(@this);
        }
    }
}
