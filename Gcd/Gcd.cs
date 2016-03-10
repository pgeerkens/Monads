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
#define PreventIncalculable
#define TrapOnes
#define FluentStyle
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using PGSolutions.Utilities.Monads;

namespace PGSolutions.Utilities.Monads.Demos {
  using PayloadMaybe  = StatePayload<Maybe<GcdStart>, Unit>;

  /// <summary>Greatest Common DIvisor demo using State Monad.</summary>
  /// <remarks>
  /// Example based on http://mvanier.livejournal.com/5846.html
  /// </remarks>
  public static class Gcd {
      static readonly CultureInfo _culture = CultureInfo.CurrentUICulture;

#if !FluentStyle
      /// <summary>TODO</summary>
      public static IO<Unit> Run(IEnumerable<GcdStart> states) {          
        states.ContractedNotNull("states");
        Contract.Ensures(Contract.Result<IO<Unit>>() != null);

        return (
            from test in Gcd_S4.GetTests(false)
            let elapsed = Readers.Timer()
            let isThird = Readers.MatchCounter(i => i==3, 1)
            let results = from start in states 
                          select new {
                              Start  = start, 
                              Result = from validated in ValidateState(start.ToMaybe()).State
                                       select test.Transform(validated).Value
                          }
            
            select ( from _   in IO.ConsoleWriteLine("{0}", test.Title)
                     from __  in ( from item in results
                                   select IO.ConsoleWriteLine(
                                       @"    GCD = {0,14} for {1}; {3,-11}; Elapsed = {2:ss\.fff} secs",
                                       item.Result.Select(
                                           r => String.Format(_culture,"{0,14:N0}", r.Gcd)
                                       ).Extract("incalculable"),
                                       item.Start,
                                       elapsed(),
                                       isThird() ? "I'm third!" : ""
                                   )
                                 ).Last()
                     from ___ in IO.ConsoleWriteLine(@"Elapsed Time: {0:ss\.ff} secs", elapsed())
                     select IO.ConsoleWriteLine()
                   )()
        ).LastUnit();
      }
#else // ComprehensionStyle
      /// <summary>TODO</summary>
      public static IO<Unit> Run(IEnumerable<GcdStart> states) {          
        states.ContractedNotNull("states");
        Contract.Ensures(Contract.Result<IO<Unit>>() != null);

        return (
            from test in Gcd_S4.GetTests(false)
            let elapsed = Readers.Timer()
            let isThird = Readers.MatchCounter(i => i==3, 1)
            let results = from start in states 
                          select new {
                              Start  = start, 
                              Result = from validated in ValidateState(start.ToMaybe()).State
                                       select test.Transform(validated).Value
                          }
            
            select ( from _   in IO.ConsoleWriteLine("{0}", test.Title)
                     from __  in ( from item in results
                                   select IO.ConsoleWriteLine(
                                       @"    GCD = {0,14} for {1}: Elapsed = {2:ss\.fff} secs; {3}",
                                       ( from r in item.Result
                                         select String.Format(_culture,"{0,14:N0}", r.Gcd)
                                       ).Extract("incalculable"),
                                       item.Start,
                                       elapsed(),
                                       isThird() ? "I'm third!" : ""
                                   )
                                 ).Last()
                     from ___ in IO.ConsoleWriteLine(@"Elapsed Time: {0:ss\.ff} secs", elapsed())
                     select IO.ConsoleWriteLine()
                   )()
        ).LastUnit();
      }
#endif

      /// <summary>TODO</summary>
      private static PayloadMaybe ValidateState(Maybe<GcdStart> start) { return
          new PayloadMaybe(
              from state in start
#if ! TrapOnes
              select state.A==1 || state.B==1 ? new GcdStart(1,1)
                   : state.A != int.MinValue
#else
              select state.A != int.MinValue
#endif
                  && state.B != int.MinValue  ? new GcdStart(Math.Abs(state.A), Math.Abs(state.B))
                   : state.A == state.B       ? new GcdStart(state.A, state.B)
                   : state.A == int.MinValue  ? new GcdStart(Math.Abs(state.A + Math.Abs(state.B)), 
                                                             Math.Abs(state.B))
#if ! PreventIncalculable
                   : state.B == int.MinValue  ? new GcdStart(Math.Abs(state.A), 
                                                             Math.Abs(state.B + Math.Abs(state.A)))
#endif
                   : Maybe<GcdStart>.Nothing
          , Unit._);
        }
  }

}
