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
using System.Linq;
using System.Reflection;

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos
{
    using StateBool = State<GcdStart, bool>;
    using StateInt = State<GcdStart, int>;
    using StateRes = State<GcdStart, GcdResult>;

    using static State;
    using static StateTransform;
    using static BindingFlags;

    /// <summary>TODO</summary>
    public static class GcdS4 { // Beware! - These must be declared & initialized in this order

        /// <summary>TODO</summary>
        /// <param name="getAll">Specify true if old implementations desired to run as well as just nes ones.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static X<IEnumerable<ITest>> GetTests(bool getAll) => (
            from item in (
                from type in new List<Type>() { typeof(GcdImperative), typeof(GcdHaskell), typeof(GcdLinq), typeof(GcdBest) }
                from info in ( type.GetMethodDescriptions(s => s.Substring(0, 3) == "Run",
                                                        Static | NonPublic | Public,
                                                        f => f?.GetValue(null) as StateRes) | null )
                select info
            )
            where getAll
               || ( item.Name != nameof(GcdHaskell)+"."+nameof(GcdHaskell.Run1)
                 && item.Name != nameof(GcdHaskell)+"."+nameof(GcdHaskell.Run2)
                 && item.Name != nameof(GcdLinq)   +"."+nameof(GcdLinq.Run1)
                 && item.Name != nameof(GcdLinq)   +"."+nameof(GcdLinq.Run2)
                  )
            select new Test(item.Details, item.Description, item.Name) as ITest
            ).AsX();

        /// <summary>TODO</summary>
        public static X<ITest> GetTest(string name) {
            return ( from test in GetTests(true) | new List<ITest>()
                     where test.Name == name
                     select test
                   ).FirstOrDefault().AsX();
        }

#region Utilities
        /// <summary>TODO</summary>
        internal delegate StateRes ResultExtractor(StateInt stateInt);

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        internal static readonly ResultExtractor GetResult = s => from gcd in s select new GcdResult(gcd);

        /// <summary>Functor to calculate GCD of two input integers.</summary>
        internal static readonly Transform<GcdStart> AlgorithmTransform = s => {
                    var x = s.A; var y = s.B;       // explicitly exposes the immutability of s.
                    return  x > y ? new GcdStart(x - y,   y  )
                          : x < y ? new GcdStart(  x,   y - x)
                                  : s;
                };

        /// <summary>State monad to calculate GCD of two input integers.</summary>
        internal static readonly StateBool AlgorithMonad = s => NewPayload(AlgorithmTransform(s), s.A != s.B);

        /// <summary>Extract either member from state as the exposed int value.</summary>
        internal static readonly StateInt GcdExtract = s => NewPayload(s, s.A);

        /// <summary>TODO</summary>
        internal struct Test : ITest {
            /// <summary>TODO</summary>
            public Test(StateRes transform, string title, string name) {
                Transform = transform; Title = title; Name = name;
            }
            /// <summary>TODO</summary>
            public StateRes Transform { get; }
            /// <summary>TODO</summary>
            public string Title { get; }
            /// <summary>TODO</summary>
            public string Name { get; }
        }
        #endregion

        /// <summary>TODO</summary>
        public static StateRes BestRun => s => GcdBest.Run(s);
    }
}
