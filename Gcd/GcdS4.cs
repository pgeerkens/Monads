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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using PGSolutions.Monads;
using System;

namespace PGSolutions.Monads.Demos {
    using StateBool     = State<GcdStart,bool>;
    using StateInt      = State<GcdStart,int>;
    using StateRes      = State<GcdStart,GcdResult>;

    using static State;
    using static StateExtensions;
    using static StateTransform;
    using static BindingFlags;
    using static GcdS4;

    /// <summary>TODO</summary>
    public interface ITest {
        /// <summary>TODO</summary>
        StateRes Transform { get; }
        /// <summary>TODO</summary>
        string   Title     { get; }
        /// <summary>TODO</summary>
        string   Name      { get; }
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    public static class GcdS4 { // Beware! - These must be declared & initialized in this order

        /// <summary>TODO</summary>
        /// <param name="getAll">Specify true if old implementations desired to run as well as just nes ones.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static X<IEnumerable<ITest>> GetTests(bool getAll) => (
            from item in (
                from type in new List<Type>() { typeof(Imperative), typeof(Haskell), typeof(Linq), typeof(Best) }
                from info in ( type.GetMethodDescriptions(s => s.Substring(0, 3) == "Run",
                                                        Static | NonPublic | Public,
                                                        f => f?.GetValue(null) as StateRes) | null )
                select info
            )
            where getAll
               || ( item.Name != nameof(Haskell)+"."+nameof(Haskell.Run1)
                 && item.Name != nameof(Haskell)+"."+nameof(Haskell.Run2)
                 && item.Name != nameof(Linq)   +"."+nameof(Linq.Run1)
                 && item.Name != nameof(Linq)   +"."+nameof(Linq.Run2)
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
        public static StateRes BestRun => s => Best.Run(s);
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    internal static class Imperative
    {
        // ~ 0.14 sec
        /// <summary>TODO</summary>
        [Description("Fully imperative; w/ substitution.")]
        public static StateRes Run1 => GetResult(new StateInt(s => {
            s.ContractedNotNull(nameof(s));
            while(s.A != s.B) s = AlgorithmTransform(s);
            return NewPayload(s, s.A);
        }));

        // ~ 0.04 sec
        /// <summary>TODO</summary>
        [Description("Fully imperative; unrolled w/ substitution.")]
        public static StateRes Run2 => GetResult(new StateInt(s => {
            s.ContractedNotNull(nameof(s));
            while(s.A != s.B)
            {
                var x = s.A; var y = s.B;
                s = x > y ? new GcdStart(x-y, x)
                  : x < y ? new GcdStart(x, y-x)
                          : s;
            }
            return NewPayload(s, s.A);
        }));
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    internal static class Haskell
    {
        #region Older Haskell implementations
        /* from http://mvanier.livejournal.com/5846.html
            gcd_s3 :: State GCDState Int
            gcd_s3 = 
              do (x, y) <- get
                 case compare x y of
                   GT -> do put (x-y,  y )
                            gcd_s3
                   LT -> do put ( x,  y-x)
                            gcd_s3
                   EQ -> return x

            run_gcd_s3 :: Int -> Int -> Int
            run_gcd_s3 x y = fst (runState gcd_s3 (x, y))
        */

        private static readonly StateBool GcdBody = new StateBool(s => NewPayload(s, s.A != s.B));
        // ~ 4.1 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel (iterative).")]
        public static StateRes Run1 => GetResult(
            GetCompose<GcdStart>(s =>
                          s.A > s.B ? Put(new GcdStart(s.A - s.B, s.B))
                        : s.A < s.B ? Put(new GcdStart(s.A, s.B - s.A))
                                    : Put(s)).Then(GcdBody)
                                             .DoWhile().Then(GcdExtract));

        // ~ 2.6 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel w/ Modify() instead of Get.Compose(s => Put(transform(s))).")]
        public static StateRes Run2 => GetResult(AlgorithmTransform.Modify().Then(GcdBody)
                                                                   .DoWhile().Then(GcdExtract));
        #endregion

        // ~ 1.5 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel w/ DoWhile().")]
        public static StateRes Run3 => GetResult(AlgorithMonad.DoWhile().Then(GcdExtract));
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    internal static class Linq
    {
        #region Older LINQ implementations
        // ~ 3.5 sec
        /// <summary>TODO</summary>
        [Description("Basic LINQ w/ Enumerate(Transform<TState>, TState) and using l'let'.")]
        public static StateRes Run1 => GetResult(new StateBool(start =>
                      (from payload in AlgorithMonad.Enumerate(start)
                       where !payload.Item2
                       select payload
                      ).First()).Then(GcdExtract));

        // ~ 2.3 sec
        /// <summary>TODO</summary>
        [Description("Better LINQ w/ Enumerate(State<TState,TValue>, TState).")]
        public static StateRes Run2 => GetResult(new StateBool(start =>
                      (from payload in AlgorithMonad.Enumerate(start)
                       where !payload.Item2
                       select payload
                      ).First()).Then(GcdExtract));
        #endregion

        // ~ 1.5 sec
        /// <summary>TODO</summary>
        [Description("Best LINQ w/ Enumerate(Transform<TState>, TState) and w/o using 'let'.")]
        public static StateRes Run3 => GetResult(new StateInt(start =>
                      (from s in AlgorithmTransform.Enumerate(start)
                       where s.A == s.B
                       select NewPayload(s, s.A)
                      ).First()));
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    internal static class Best
    {
        // ~ 1.0 sec
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [Description("Optimized DoWhile().")]
        public static StateRes Run => GetResult(AlgorithmTransform.DoWhile(s => s.A != s.B)
                                                                  .Then(GcdExtract));
    }
}
