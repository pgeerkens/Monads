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
  /// <summary>TODO</summary>
  /// <typeparam name="TState">Type of the state which this delegate transforms.</typeparam>
  public delegate TState Transform<TState>(TState s);

  /// <summary>TODO</summary>
  [Pure]public static class TransformExtensions {
      /// <summary>TODO</summary>
      public static State<TState,Unit>                  DoWhile<TState>( this
          Transform<TState> @this,
          Func<TState,bool> predicate
      ) {
          @this.ContractedNotNull("this");
          Contract.Ensures(Contract.Result<State<TState,Unit>>() != null);

          return s => {
              while (predicate(s)) { s = @this(s); }
              return new StatePayload<TState,Unit>(s,Unit._);
          };
      }

      /// <summary>Generates an unending stream of successive TState objects.</summary>
      public static IEnumerable<TState>                 Enumerate<TState>( this
          Transform<TState> @this,
          TState startState
      ) {
          @this.ContractedNotNull("this");
          startState.ContractedNotNull("startState");
          Contract.Ensures(Contract.Result<IEnumerable<TState>>() != null);

          while (true) yield return (startState = @this(startState));
      }

      /// <summary>Puts the transformed state and returns the original.</summary>
      public static State<TState,TState>                Modify<TState>( this
          Transform<TState> @this
      ) {
          @this.ContractedNotNull("this");
          Contract.Ensures(Contract.Result<State<TState,TState>>() != null); 

          return State.Modify(@this);
      }
  }
}
