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
using System.Diagnostics.CodeAnalysis;

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>Core Monadic functionality for State, as Extension methods</summary>
    public static class StateTransform {
        /// <summary>TODO</summary>
        /// <typeparam name="TState">Type of the state which this delegate transforms.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public delegate TState Transform<TState>(TState s);

        /// <summary>TODO</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="predicate">todo: describe predicate parameter on DoWhile</param>
        public static State<TState, Unit>          DoWhile<TState>(this
            Transform<TState> @this, 
            Predicate<TState> predicate
        ) {
            @this.ContractedNotNull(nameof(@this));
            Ensures(Result<State<TState, Unit>>() != null);

            return s => { while (predicate(s)) { s = @this(s); }
                          return StructTuple.New(s, Unit._);
                        };
        }

        /// <summary>Generates an unending stream of successive TState objects.</summary>
        /// <param name="this">The start state for this transformation.</param>
        /// <param name="startState">todo: describe startState parameter on Enumerate</param>
        public static IEnumerable<TState>           Enumerate<TState>(this
            Transform<TState> @this,
            TState startState
        ) {
            @this.ContractedNotNull(nameof(@this));
            startState.ContractedNotNull(nameof(startState));
            Ensures(Result<IEnumerable<TState>>() != null);

            while (true) yield return (startState = @this(startState));
        }

        /// <summary>Puts the transformed state and returns the original state.</summary>
        /// <remarks>Optimized implementation of Get.Compose(s => Put(transform(s))).</remarks>
        public static State<TState,TState>         Modify<TState>(this Transform<TState> transform) {
            transform.ContractedNotNull(nameof(transform));
            Ensures(Result<State<TState,TState>>() != null); 

            return new State<TState,TState>(
                 s => StructTuple.New(transform(s), s)
            );
        }
    }
}
