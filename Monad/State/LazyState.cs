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

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>The State monad.</summary>
    /// <typeparam name="TState">Type of the internal state threaded by this instance.</typeparam>
    /// <typeparam name="TValue">Type of the calculated value exposed by this instance.</typeparam>
    public delegate StatePayload<TState,TValue>         LazyState<TState,TValue>(TState s);

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    [Pure]
    public static class LazyState {
        #region m.Select()
        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static X<LazyState<TState, TResult>>     Select<TState, TValue, TResult>(this
            X<LazyState<TState, TValue>> @this,
            Func<TValue, TResult> selector
        ) => from t in @this from r in t.Select(selector) select r;

        /// <summary>Optimized implementation of operator (liftM): liftM f m = m >>= (\x -> return (f x)).</summary>
        public static X<LazyState<TState, TResult>>     Select<TState, TValue, TResult>(this
            LazyState<TState, TValue> @this,
            Func<TValue, TResult> selector
        ) => selector!=null ? @this.SelectMany<TState,TValue,TResult>(x => s => StatePayload.New(s,selector(x)))
                            : (X<LazyState<TState, TResult>>)null;
        #endregion
        #region m.SelectMany(,)
        /// <summary>LINQ-compatible alias for join.</summary>
        public static X<LazyState<TState, TResult>>     SelectMany<TState, TValue, T, TResult>(this
            X<LazyState<TState, TValue>> @this,
            X<Func<TValue, LazyState<TState, T>>> selector,
            Func<TValue, T, TResult> projector
        ) => from t in @this from s in selector from r in t.SelectMany(s, projector) select r;
        /// <summary>LINQ-compatible alias for join.</summary>
        public static X<LazyState<TState,TResult>>      SelectMany<TState,TValue,T,TResult> (this
            X<LazyState<TState,TValue>> @this,
            Func<TValue, LazyState<TState,T>> selector,
            Func<TValue, T, TResult> projector
        ) => from t in @this from r in t.SelectMany(selector,projector) select r;
        /// <summary>LINQ-compatible alias for join.</summary>
        /// <remarks>
        ///     return  @this.SelectMany(          aval =>      // Always available from Bind()
        ///             selector(aval).SelectMany( bval =>
        ///             new State{TState,TResult}( s    => 
        ///             new StatePayload{TState,TResult}(s,projector(aval,bval)) ) ) );
        /// </remarks>
        public static X<LazyState<TState,TResult>>      SelectMany<TState,TValue,T,TResult> (this
            LazyState<TState,TValue> @this,
            Func<TValue, LazyState<TState,T>> selector,
            Func<TValue, T, TResult> projector
        ) => @this!=null && selector!=null && projector!=null
            ? s => @this(s).Selector1(selector).Selector2(projector)
            : (LazyState<TState,TResult>)null;

        static Tuple<TValue,StatePayload<TState,TSelector>> Selector1<TState,TValue,TSelector>(this StatePayload<TState,TValue> @this,
            Func<TValue, LazyState<TState,TSelector>> selector
        ) => Tuple.Create(@this.Value, selector(@this.Value)(@this.State));
        static StatePayload<TState,TResult> Selector2<TState,TValue,TSelector,TResult>(this Tuple<TValue,StatePayload<TState,TSelector>> @this,
            Func<TValue, TSelector, TResult> projector
        ) => StatePayload.New(@this.Item2.State, projector(@this.Item1,@this.Item2.Value));
        #endregion
        #region m.SelectMany()
        /// <summary>Implementation of Bind operator: (>>=): m a -> (a -> m b) -> m b.</summary>
        public static X<LazyState<TState,TResult>>      SelectMany<TState,TValue,TResult> (this
            LazyState<TState,TValue> @this,
            Func<TValue,LazyState<TState,TResult>> selector
        ) => @this.AsX().SelectMany(selector.AsX());
        /// <summary>Implementation of Bind operator: (>>=): m a -> (a -> m b) -> m b.</summary>
        public static X<LazyState<TState,TResult>>      SelectMany<TState,TValue,TResult> (this
            X<LazyState<TState,TValue>> @this,
            Func<TValue,LazyState<TState,TResult>> selector
        ) => @this.SelectMany(selector.AsX());
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
        public static X<LazyState<TState, TResult>> SelectMany<TState, TValue, TResult>(this
            X<LazyState<TState, TValue>> @this,
            X<Func<TValue, LazyState<TState, TResult>>> selector
        ) => from t in @this from s in selector from r in t.SelectMany(s, Functions.Second) select r;
        #endregion

        #region Miscellaneous
        /// <summary>TODO</summary>
         public static StatePayload<TState,TValue>? Apply<TState,TValue>(this X<LazyState<TState,TValue>> @this, TState s) =>
            @this.SelectMany<LazyState<TState,TValue>, StatePayload<TState,TValue>>(m => m.Invoke(s));

        /// <summary>Creates a new instance of <see cref="LazyState{TState, TValue}"/> from the supplied <paramref name="this"/></summary>
        /// <param name="this"> the <typeparamref name="TValue"/> value for the new instance.</param>
        public static LazyState<TState, TValue>         ToLazyState<TState, TValue>(this TValue @this) =>
            s => StatePayload.New(s,@this);

        /// <summary>Iterate the supplied <see cref="LazyState{TState,TValue}"/> until the calculated value is false.</summary>
        public static X<LazyState<TState,bool>>         DoWhile<TState>(this
            LazyState<TState,bool> @this
        ) => (LazyState<TState,bool>)(s => {
                StatePayload<TState,bool> payload;
                do { payload = @this(s); s = payload.State; } while (payload.Value);
                return payload;
             } );

        /// <summary>Generates an unending stream of successive StructTuple{TState,T} objects.</summary>
        /// <remarks>The caller is responsible for externally terminating the loop.</remarks>
        public static IEnumerable<StatePayload<TState,TValue>>  Enumerate<TState,TValue>(this
            LazyState<TState,TValue> @this,
            TState startState
        ) {
            Ensures(Result<IEnumerable<StatePayload<TState,TValue>>>() != null);

            var tuple = StatePayload.New(startState,default(TValue));
            while (true) yield return tuple = @this(tuple.State);
        }

        /// <summary>'Gets' the current state, as both State and Value.</summary>
        public static LazyState<TState, TState> Get<TState>() => s => StatePayload.New(s,s);
        /// <summary>'Puts' the supplied state, resturning a Unit.</summary>
        public static LazyState<TState,Unit>    Put<TState>(TState state) => s => StatePayload.New(state, Unit._);
        #endregion
    }
}
