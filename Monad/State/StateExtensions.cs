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

using PGSolutions.Utilities.Monads;

namespace PGSolutions.Utilities.Monads {

  /// <summary>LINQ-compatible extension methods for <see cref="StateTuple<TState,TValue>"/>.</summary>
  [Pure]
  public static class StateExtensions {

    ///// <summary>LINQ-compatible alias for <b>liftM/fmap</b>: (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
    //public static State<TState,TResult>           Select<TState,TValue,TResult>( this
    //    State<TState, TValue> @this,
    //    Func<TValue, TResult> projector
    //) {
    //    @this.ContractedNotNull("this");
    //    projector.ContractedNotNull("projector");
    //    Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

    //    return State.Select(@this, projector);
    //}

    ///// <summary>LINQ-compatible alias for <b>Bind</b>: (>>=): m a -> (a -> m b) -> m b.</summary>
    //public static State<TState,TResult>           SelectMany<TState,TValue,TResult>( this
    //    State<TState, TValue> @this,
    //    Func<StatePayload<TState,TValue>, State<TState,TResult>> selector
    //) {
    //    @this.ContractedNotNull("this");
    //    selector.ContractedNotNull("selector");
    //    Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

    //    return State.SelectMany(@this, selector);
    //}

    ///// <summary>LINQ-compatible alias for ?join?.</summary>
    //public static State<TState,TResult>           SelectMany<TState,TValue,T,TResult>( this
    //    State<TState, TValue> @this,
    //    Func<StatePayload<TState,TValue>, State<TState,T>> selector,
    //    Func<TValue,T, TResult> projector
    //) {
    //    @this.ContractedNotNull("this");
    //    selector.ContractedNotNull("selector");
    //    projector.ContractedNotNull("projector");
    //    Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

    //    return State.SelectMany(@this, selector, projector);
    //}

    /// <summary>Implementation of <b>compose</b>: (>=>): f >=> g = \x -> (f x >>= g). </summary>
    /// <remarks> Optimized implementation of:
    ///         return this.Bind(t => follower(t.Item2));
    /// </remarks>
    public static State<TState,TResult>           Compose<TState,TValue,TResult> ( this
        State<TState, TValue> @this,
        Func<TState, State<TState,TResult>> follower
    ) {
        @this.ContractedNotNull("this");
        follower.ContractedNotNull("follower");
        Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

        return new State<TState,TResult>( s => follower(s)(@this(s).State) );
    }

    /// <summary>Implementation of <b>then</b>: (>>):  mv1 >> mv2  =  mv1 >>= (\_ -> mv2)</summary>
    /// <remarks> Optimized implementation of:
    ///         return @this.Bind(t => follower);
    /// or
    ///         return @this.Then(s => follower);
    /// </remarks>
    public static State<TState,TResult>           Then<TState,TValue,TResult>( this
        State<TState, TValue> @this,
        State<TState, TResult> follower
    ) {
        @this.ContractedNotNull("this");
        follower.ContractedNotNull("follower");
        Contract.Ensures(Contract.Result<State<TState,TResult>>() != null);

        return new State<TState,TResult>( s => follower(@this(s).State) );
    }
  }
}
