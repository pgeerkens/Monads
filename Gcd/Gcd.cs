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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos {
    using static IOMonads;
    using static FormattableString;
    using static Extensions;

#if GcdStartAsClass
    using PayloadMaybe  = ValueTuple<X<GcdStart>, Unit>;
    using GcdStartType  = X<GcdStart>;

    internal static class Extensions {
        public const string GcdStartTYpe = "GcdStart as class.";
        public static X<T> ToMaybe<T>(this T gcdStart) where T:class => gcdStart.AsX();
#else
    using PayloadMaybe  = ValueTuple< GcdStart?,  Unit>;
    using GcdStartType  = Nullable<GcdStart>;       // AKA: GcdStart?

    internal static class Extensions {
        public const string GcdStartTYpe = "GcdStart as struct.";
        public static  T?  ToMaybe<T>(this T gcdStart) where T:struct => gcdStart.ToNullable();
#endif
        public static readonly GcdStartType GcdStartDefault = default(GcdStartType);
    }

    /// <summary>Greatest Common Divisor demo using State Monad.</summary>
    /// <remarks>
    /// Example based on http://mvanier.livejournal.com/5846.html
    /// </remarks>
    public static class Gcd {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Unit? Run(this X<IList<GcdStart>> maybeStates) =>
            maybeStates.SelectMany<IList<GcdStart>,Unit>(states =>
                  states.AsX().ForAllTests().LastOrDefault()
            );

        private static IEnumerable<Unit> ForAllTests(this X<IList<GcdStart>> states) =>
            from test in  GcdS4.GetTests(false) | new List<ITest>()
            let timer   = Readers.Timer()
            let isThird = Readers.MatchCounter(i => i==3, 1)
            select ( from _   in ConsoleWriteLine("{0} - {1}", GcdStartTYpe, test.Title)
                     from __  in (
                            #if NotComprehensiveSyntax
                                   states.ForThisTest(test, timer, isThird)
                                         .SelectMany(e=>e.Last())
                            #else
                                   from e in states.ForThisTest(test, timer, isThird)
                                   from i in e.Last()
                                   select i
                            #endif
                                 )
                     from ___ in ConsoleWriteLine(@"Elapsed Time: {0:ss\.ff} secs", timer())
                     from _x_ in ConsoleWriteLine()
                     select Unit.Empty
            ).Invoke();

        private static X<IEnumerable<IO<Unit>>> ForThisTest(this X<IList<GcdStart>> startStates, 
            ITest           test, 
            Func<TimeSpan>  elapsed, 
            Func<bool>      isThird
        )=>from states in startStates
           select ( from state in states
                    select new {
                        Start  = state,
                        Result = from validated in state.ToMaybe().ValidateState().Item1
                                 select test.Transform(validated).Item2
                    } into item
                    select ConsoleWriteLine(
                        @"    GCD = {0,14} for {1}: Elapsed = {2:ss\.fff} secs; {3}",
                        ( from r in item.Result
                    #if true
                          from s in Invariant($"{r.Gcd,14:N0}").AsX()
                          select s
                        ) | "incalculable",
                    #elif GcdStartAsClass
                          select Invariant($"{r.Gcd,14:N0}")
                        ) | "incalculable",
                    #else
                          select Invariant($"{r.Gcd,14:N0}").AsX()
                        ) ?? "incalculable",
                    #endif
                        item.Start,
                        elapsed(),
                        isThird() ? "I'm third!" : ""
                  ) );

        /// <summary>Return a pair of positive integers with the same GCD as the supplied parameters.</summary>
        private static PayloadMaybe ValidateState(this GcdStartType start) {
            return State.NewPayload(
                from state in start
                from x in state.A == 1
                       || state.B == 1            ? new GcdStart(1, 1).ToMaybe()
                        : state.A != int.MinValue
                       && state.B != int.MinValue ? new GcdStart(Math.Abs(state.A), Math.Abs(state.B)).ToMaybe()
                        : state.A ==      state.B ? new GcdStart(state.A, state.B).ToMaybe()
                        : state.A == int.MinValue ? new GcdStart(Math.Abs(state.A + Math.Abs(state.B)),
                                                                 Math.Abs(state.B)
                                                                ).ToMaybe()
                    #if PreventIncalculable
                        : state.B == int.MinValue ? new GcdStart(Math.Abs(state.A),
                                                                 Math.Abs(state.B + Math.Abs(state.A))
                                                                ).ToMaybe()
                    #endif
                        : GcdStartDefault
                select x,
                Unit.Empty);
        }
    }
}
