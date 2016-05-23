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
#define FluentStyle
//#define PreventIncalculable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using PGSolutions.Utilities.Monads;

namespace PGSolutions.Utilities.Monads.Demos {
    using PayloadMaybe  = StatePayload<Maybe<GcdStart>, Unit>;
    using static IOMonads;

    /// <summary>Greatest Common DIvisor demo using State Monad.</summary>
    /// <remarks>
    /// Example based on http://mvanier.livejournal.com/5846.html
    /// </remarks>
    public static class Gcd {
        static readonly CultureInfo _culture = CultureInfo.CurrentUICulture;

        /// <summary>TODO</summary>
        public static Maybe<IO<Unit>> Run2(Maybe<IReadOnlyList<GcdStart>> maybeStates) =>
            from states in maybeStates select Run(states);

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static IO<Unit> Run(IReadOnlyList<GcdStart> states) {
            states.ContractedNotNull("states");

            return (
                from test in Gcd_S4.GetTests(false) | new List<ITest>()
                let elapsed = Readers.Timer()
                let isThird = Readers.MatchCounter(i => i==3, 1)
                select ( from _   in ConsoleWriteLine("{0}", test.Title)
                         from __  in ( from start in states
                                       select new {
                                          Start = start,
                                          Result = from validated in ValidateState(start.ToMaybe()).State
                                                   select test.Transform.Invoke(validated).Value
                                       } into item
                                       select ConsoleWriteLine(
                                           @"    GCD = {0,14} for {1}: Elapsed = {2:ss\.fff} secs; {3}",
                                           ( from r in item.Result
                                             select String.Format(_culture,"{0,14:N0}", r.Gcd)
                                           ) | "incalculable",
                                           item.Start,
                                           elapsed(),
                                           isThird() ? "I'm third!" : ""
                                       )
                                     ).Last()
                         from ___ in ConsoleWriteLine(@"Elapsed Time: {0:ss\.ff} secs", elapsed())
                         select ConsoleWriteLine()
                       ).Invoke()
            ).LastOrDefault();
          }

        /// <summary>Return a pair of positive integers with the same GCD as the supplied parameters.</summary>
        private static PayloadMaybe ValidateState(Maybe<GcdStart> start) {
            return new PayloadMaybe(
                from state in start
                from x in state.A == 1
                       || state.B == 1            ? new GcdStart(1, 1)
                        : state.A != int.MinValue
                       && state.B != int.MinValue ? new GcdStart(Math.Abs(state.A), Math.Abs(state.B))
                        : state.A == state.B      ? new GcdStart(state.A, state.B)
                        : state.A == int.MinValue ? new GcdStart(Math.Abs(state.A + Math.Abs(state.B)),
                                                                 Math.Abs(state.B))
#if PreventIncalculable
                        : state.B == int.MinValue ? new GcdStart(Math.Abs(state.A), 
                                                                 Math.Abs(state.B + Math.Abs(state.A)))
#endif
                        : Maybe<GcdStart>.Nothing
                select x,
                Unit._);
        }
    }
}
