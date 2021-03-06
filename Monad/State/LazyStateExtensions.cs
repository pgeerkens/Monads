﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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

namespace PGSolutions.Monads {
    /// <summary>Extension methods for <see cref="LazyState{TState,TValue}"/>.</summary>
    public static partial class LazyStateExtensions {

        /// <summary>Implementation of <b>compose</b>: (>=>): f >=> g = \x -> (f x >>= g). </summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="follower">todo: describe follower parameter on Compose</param>
        /// <remarks> Optimized implementation of:
        ///         return this.Bind(t => follower(t.Item2));
        /// </remarks>
        public static LazyState<TState,TResult>     Compose<TState,TValue,TResult> ( this
            LazyState<TState, TValue> @this,
            Func<TState, LazyState<TState,TResult>> follower
        ) {
            follower.ContractedNotNull(nameof(follower));

            return new LazyState<TState,TResult>( s => follower(s)(@this(s).State) );
        }

        /// <summary>Implementation of <b>then</b>: (>>):  mv1 >> mv2  =  mv1 >>= (\_ -> mv2)</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="follower">todo: describe follower parameter on Then</param>
        /// <remarks> Optimized implementation of:
        ///         return @this.Bind(t => follower);
        /// or
        ///         return @this.Then(s => follower);
        /// </remarks>
        public static LazyState<TState,TResult>     Then<TState,TValue,TResult>( this
            LazyState<TState, TValue> @this,
            LazyState<TState, TResult> follower
        ) {
            @this.ContractedNotNull(nameof(@this));
            follower.ContractedNotNull(nameof(follower));

            return s => follower(@this(s).State);
        }

        /// <summary>TODO</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="s">todo: describe s parameter on Value</param>
        public static TValue    Value<TState, TValue>(this LazyState<TState,TValue> @this, TState s) {
            @this.ContractedNotNull(nameof(@this));

            return @this.Run(s).Value;
        }

        /// <summary>TODO</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="s">todo: describe s parameter on State</param>
        public static TState    State<TState, TValue>(this LazyState<TState,TValue> @this, TState s) {
            @this.ContractedNotNull(nameof(@this));

            return @this.Run(s).State;
        }

        /// <summary>TODO</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="s">todo: describe s parameter on State</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static LazyPayload<TState,TValue> Run<TState, TValue>(this LazyState<TState, TValue> @this, TState s) {
            @this.ContractedNotNull(nameof(@this));

            return  @this(s);;
        }
    }
}
