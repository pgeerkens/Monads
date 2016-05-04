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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using PGSolutions.Utilities.Monads;

namespace PGSolutions.Utilities.Monads.Demos {
    using StateBool    = State<GcdStart, bool>;
    using PayloadBool  = StatePayload<GcdStart, bool>;
    using StateInt     = State<GcdStart, int>;
    using PayloadInt   = StatePayload<GcdStart, int>;
    using StateRes     = State<GcdStart, GcdResult>;

    /// <summary>TODO</summary>
    public interface ITest {
        /// <summary>TODO</summary>
        StateRes Transform { get; }
        /// <summary>TODO</summary>
        string Title { get; }
    }

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [Pure]
    public static class Gcd_S4 { // Beware! - These must be declared & initialized in this order
        /// <summary>TODO</summary>
        public static IEnumerable<ITest> GetTests(bool getAll) {
            Contract.Ensures(Contract.Result<IEnumerable<ITest>>() != null);

            var list =  ( from test2 in new List<StateRes>() {
                             Imperative.Run1, Imperative.Run2,
                             Haskell.Run1,    Haskell.Run2,  Haskell.Run3,
                             Linq.Run1,       Linq.Run2,     Linq.Run3,
                             Best.Run
                           }
                          select new Test(test2,GetTitle(test2)) as ITest
                        );

            if ( ! getAll) 
                list = ( from item in list
                         where item.Transform != Haskell.Run1
                            && item.Transform != Haskell.Run2
                            && item.Transform != Linq.Run1
                            && item.Transform != Linq.Run2
                         select item);

            return list.ToList().AsReadOnly();
        }

        /// <summary>TODO</summary>
        public static string GetTitle(StateRes transform) {
            const BindingFlags bindFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            transform.ContractedNotNull("transform");
            Contract.Ensures(Contract.Result<string>() != null);

            var result  = ( from @class  in typeof(Gcd_S4).GetNestedTypes(bindFlags)
                            from fields in @class.GetFields(bindFlags)
                            from atts in fields.CustomAttributes
                            where atts.ConstructorArguments.Count > 0
                              &&  ReferenceEquals(fields.GetValue(typeof(StateRes)), transform)
                            select atts.ConstructorArguments[0].Value as string
                            ).FirstOrDefault()
                            ?? "";
            return result;
        }

        #region Utilities
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Func<StateInt, StateRes> ToStateRes = (transform) =>
#if ! true
            transform.Select( gcd => new GcdResult(gcd) );
#else
            from gcd in transform select new GcdResult(gcd);
#endif
        /// <summary>Functor to calculate GCD of two input integers.</summary>
        private static readonly Transform<GcdStart> AlgorithmTransform = s => {
                                      var x = s.A; var y = s.B;
                                      return  x > y ? new GcdStart(x - y,   y  )
                                            : x < y ? new GcdStart(  x,   y - x)
                                                    : s;
                                    };
        /// <summary>State monad to calculate GCD of two input integers.</summary>
        private static readonly StateBool AlgorithmState = s => {
                                      var x = s.A; var y = s.B;
                                      return  new PayloadBool(
                                              x > y ? new GcdStart(x - y,   y  )
                                            : x < y ? new GcdStart(  x,   y - x)
                                                    : s
                                            , x != y);
                                    };
        /// <summary>Extract either member from state as the exposed int value.</summary>
        private static readonly StateInt GcdExtract = s => new PayloadInt(s, s.A);
        #endregion

        /// <summary>TODO</summary>
        [Pure]
        internal static class Haskell {
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

          // ~ 4.1 sec
        private static readonly StateBool GcdBody = s => new PayloadBool(s, s.A != s.B);
        private static readonly StateInt _run1 =
            State.GetCompose<GcdStart>(s =>
                          s.A > s.B ? State.Put(new GcdStart(s.A - s.B, s.B))
                        : s.A < s.B ? State.Put(new GcdStart(s.A, s.B - s.A))
                                    : State.Put(s)).Then(GcdBody)
                                                   .DoWhile().Then(GcdExtract);
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [Description("Straight Haskel (iterative).")]
        public static readonly StateRes Run1 = ToStateRes(_run1);

          // ~ 2.6 sec
        private static readonly StateInt _run2 = AlgorithmTransform.Modify().Then(GcdBody)
                                                                   .DoWhile().Then(GcdExtract);
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [Description("Straight Haskel w/ Modify() instead of Get.Compose(s => Put(transform(s))).")]
        public static readonly StateRes Run2 = ToStateRes(_run2);
        #endregion

              // ~ 1.5 sec
            private static readonly StateInt _run3 = AlgorithmState.DoWhile().Then(GcdExtract);
            /// <summary>TODO</summary>
            [Description("Straight Haskel w/ DoWhile().")]
            public static readonly StateRes Run3 = ToStateRes(_run3);
        }

        /// <summary>TODO</summary>
        [Pure]
        internal static class Linq {
            #region Older LINQ implementations
              // ~ 3.5 sec
            private static readonly StateInt _run1 = start =>
                          ( from s in AlgorithmTransform.Enumerate(start)
                            let A = s.A
                            let B = s.B
                            where A == B
                            select new PayloadInt(s, A)
                          ).First();
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Basic LINQ w/ Enumerate(Transform<TState>, TState) and using l'let'.")]
            public static readonly StateRes Run1 = ToStateRes(_run1);

              // ~ 2.3 sec
            private static readonly StateInt _run2 = new StateBool(start =>
                          ( from payload in AlgorithmState.Enumerate(start)
                            where !payload.Value
                            select payload
                          ).First()).Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Better LINQ w/ Enumerate(State<TState,TValue>, TState).")]
            public static readonly StateRes Run2 = ToStateRes(_run2); 
            #endregion

               // ~ 1.5 sec
            private static readonly StateInt _run3 = start =>
                          ( from s in AlgorithmTransform.Enumerate(start)
                            where s.A == s.B
                            select new PayloadInt(s, s.A)
                          ).First();
            /// <summary>TODO</summary>
            [Description("Best LINQ w/ Enumerate(Transform<TState>, TState) and w/o using 'let'.")]
            public static readonly StateRes Run3 = ToStateRes(_run3);
        }

        /// <summary>TODO</summary>
        private struct Test : ITest {
            /// <summary>TODO</summary>
            public Test(StateRes transform, string title) { _transform = transform; _title = title; }
            /// <summary>TODO</summary>
            public StateRes       Transform  { get { return _transform;} } readonly StateRes _transform;
            /// <summary>TODO</summary>
            public string         Title      { get { return _title;    } } readonly string   _title;
        }

        /// <summary>TODO</summary>
        [Pure]
        internal static class Imperative {

              // ~ 0.91 sec
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            [Description("Fully imperative; w/o substitution.")]
            public static readonly StateRes Run1 = Gcd_S4.ToStateRes(_run1);
            private static PayloadInt _run1(GcdStart state) {
                while (state.A != state.B) {
                    state  = state.A > state.B ? new GcdStart(state.A - state.B,      state.A     )
                           : state.A < state.B ? new GcdStart(     state.A,      state.B - state.A)
                                               : state;
                  }
              return new PayloadInt(state, state.A);
            }

              // ~ 0.60 sec
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            [Description("Fully imperative; w/ substitution.")]
            public static readonly StateRes Run2 = Gcd_S4.ToStateRes(_run2);
            private static PayloadInt _run2(GcdStart state) {
                while (state.A != state.B) {
                    var x = state.A; var y = state.B;

                    state  =    x    >    y    ? new GcdStart(   x    -    y,            x        )
                           :    x    <    y    ? new GcdStart(        x,            y    -    x   )
                                               : state;
                }
                return new PayloadInt(state, state.A);
            }
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [Pure]
        public static class Best {
              // ~ 1.0 sec
            private static readonly StateInt _run = AlgorithmTransform.DoWhile(s => s.A != s.B)
                                                                      .Then(GcdExtract);
                /// <summary>TODO</summary>
                [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
                [Description("Optimized DoWhile().")]
            public static readonly StateRes Run = ToStateRes(_run);
        }
    }
}
