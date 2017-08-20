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

namespace PGSolutions.Monads {
    /// <summary>Extension methods for <see cref="ValueTuple{TState,TValue}"/>.</summary>
    public static partial class StateExtensions {

        /// <summary>Implementation of <b>compose</b>: (>=>): f >=> g = \x -> (f x >>= g). </summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="follower">todo: describe follower parameter on Compose</param>
        /// <remarks> Optimized implementation of:
        ///         return this.Bind(t => follower(t.Item2));
        /// </remarks>
        public static State<TState,TResult>     Compose<TState,TValue,TResult> ( this
            State<TState, TValue> @this,
            Func<TState, State<TState,TResult>> follower
        ) {
            follower.ContractedNotNull(nameof(follower));

            return new State<TState,TResult>( s => follower(s)(@this(s).Item1) );
        }

        /// <summary>Implementation of <b>then</b>: (>>):  mv1 >> mv2  =  mv1 >>= (\_ -> mv2)</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="follower">todo: describe follower parameter on Then</param>
        /// <remarks> Optimized implementation of:
        ///         return @this.Bind(t => follower);
        /// or
        ///         return @this.Then(s => follower);
        /// </remarks>
        public static State<TState,TResult>     Then<TState,TValue,TResult>( this
            State<TState, TValue> @this,
            State<TState, TResult> follower
        ) => @this == null || follower == null ? (State<TState,TResult>)null
                                               : s => follower(@this(s).Item1);

        /// <summary>Return the <typeparamref name="TValue"/> value calculated against the supplied <typeparamref name="TState"/> value.</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="s">The starting <typeparamref name="TState"/> value for this transformation.</param>
        /// <typeparam name="TState">The State type for this state-monad instance.</typeparam>
        /// <typeparam name="TValue">The Value type for this state-monad instance.</typeparam>
        public static TValue?    Value<TState,TValue>(this
            State<TState,TValue> @this,
            TState s
        ) where TValue:struct => from value in @this.Run(s) select value.Item2;

        /// <summary>Return the <typeparamref name="TState"/> value calculated against the supplied <typeparamref name="TState"/> value.</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="s">The starting <typeparamref name="TState"/> value for this transformation.</param>
        /// <typeparam name="TState">The State type for this state-monad instance.</typeparam>
        /// <typeparam name="TValue">The Value type for this state-monad instance.</typeparam>
        public static TState?    State<TState,TValue>(this
            State<TState,TValue> @this,
            TState s
        ) where TState:struct => from value in @this.Run(s) select value.Item1;

        private static ValueTuple<TState,TValue>        Default<TState,TValue>() => default(ValueTuple<TState,TValue>);

        /// <summary>Evaluate this state-monad instance against the supplied <typeparamref name="TState"/> value.</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="state">todo: describe s parameter on State</param>
        /// <typeparam name="TState">The State type for this state-monad instance.</typeparam>
        /// <typeparam name="TValue">The Value type for this state-monad instance.</typeparam>
        public static ValueTuple<TState,TValue>?   Run<TState,TValue>(this
            State<TState,TValue> @this,
            TState state
        ) => state == null ? (ValueTuple<TState,TValue>?)null 
                           : (@this?.Invoke(state) ?? Default<TState,TValue>());
    }

    /// <summary>TODO</summary>
    public static class MaybeStateExtensions {
        /// <summary>Implementation of <b>then</b>: (>>):  mv1 >> mv2  =  mv1 >>= (\_ -> mv2)</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="follower">todo: describe follower parameter on Then</param>
        /// <remarks> Optimized implementation of:
        ///         return @this.Bind(t => follower);
        /// or
        ///         return @this.Then(s => follower);
        /// </remarks>
        public static X<State<TState,TResult>>     Then<TState,TValue,TResult>( this
            X<State<TState,TValue>> @this,
            X<State<TState,TResult>> follower
        ) => from me in @this from f in follower select me.Then(f);
    }
}
