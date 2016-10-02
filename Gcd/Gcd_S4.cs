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

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos {
    using StateBool     = State<GcdStart,bool>;
    using StateInt      = State<GcdStart,int>;
    using StateRes      = State<GcdStart,GcdResult>;

    using Payload       = StructTuple;
    using PayloadInt    = StructTuple<GcdStart,int>;

    using static State;
    using static StateExtensions;
    using static StateTransform;

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
    [Pure]
    public static class Gcd_S4 { // Beware! - These must be declared & initialized in this order

        const BindingFlags bindFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>TODO</summary>
        /// <param name="getAll">Specify true if old implementations desired to run as well as just nes ones.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static X<IEnumerable<ITest>> GetTests(bool getAll) =>
            ( from item in typeof(Gcd_S4).GetMethodDescriptions(s => s.Substring(0, 3) =="Run")
              where getAll
                 || ( item.Name != nameof(Haskell)+"."+nameof(Haskell.Run1)
                   && item.Name != nameof(Haskell)+"."+nameof(Haskell.Run2)
                   && item.Name != nameof(Linq)+"."+nameof(Linq.Run1)
                   && item.Name != nameof(Linq)+"."+nameof(Linq.Run2)
              )
              select new Test(item.Transform, item.Description, item.Name) as ITest
            ).AsX();

        /// <summary>TODO</summary>
        internal static IList<MethodDescriptor> GetMethodDescriptions(this Type type, Predicate<string> predicate) =>
            type==null ? new List<MethodDescriptor>().AsReadOnly() :
            ( from @class in type?.GetNestedTypes(bindFlags)
              from field  in @class?.GetFields(bindFlags)
              from atts   in field?.CustomAttributes
              where predicate(field?.Name ?? "") //?? false
                 && atts?.AttributeType.Name == "DescriptionAttribute"
              select new MethodDescriptor {
                    Name        = @class?.Name + "." + field?.Name,
                    Description = atts?.ConstructorArguments[0].Value as string,
                    Transform   = field?.GetValue(null) as StateRes
            } ).ToList().AsReadOnly();

        /// <summary>TODO</summary>
        internal class MethodDescriptor {
            /// <summary>TODO</summary>
            public string   Name        { get; internal set; }
            /// <summary>TODO</summary>
            public string   Description { get; internal set; }
            /// <summary>TODO</summary>
            public StateRes Transform   { get; internal set; }
        }

        /// <summary>TODO</summary>
        public static X<ITest> GetTest(string name) {
            return ( from test in GetTests(true) | new List<ITest>()
                     where test.Name == name
                     select test
                   ).FirstOrDefault().AsX();
        }

#region Utilities
        /// <summary>TODO</summary>
        delegate StateRes ResultExtractor(StateInt stateInt);

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        static readonly ResultExtractor GetResult = (s) => from gcd in s select new GcdResult(gcd);

        /// <summary>Functor to calculate GCD of two input integers.</summary>
        static readonly Transform<GcdStart> AlgorithmTransform = s => {
                              var x = s.A; var y = s.B; // explicitly exposes the immutability of s.
                              return  x > y ? new GcdStart(x - y,   y  )
                                    : x < y ? new GcdStart(  x,   y - x)
                                            : s;
                            };
        /// <summary>State monad to calculate GCD of two input integers.</summary>
        static readonly StateBool AlgorithmState = s => {
                              var x = s.A; var y = s.B; // explicitly exposes the immutability of s.
                              return Payload.New(
                                      x > y ? new GcdStart(x - y,   y  )
                                    : x < y ? new GcdStart(  x,   y - x)
                                            : s
                                    , x != y);
                            };
        /// <summary>Extract either member from state as the exposed int value.</summary>
        static readonly StateInt GcdExtract = s => Payload.New(s, s.A);

        /// <summary>TODO</summary>
        private struct Test : ITest {
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
        [Pure]
        internal static class Imperative {

            // ~ 0.91 sec
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            [Description("Fully imperative; w/o substitution.")]
            public static readonly StateRes Run1 = GetResult(new StateInt(s=>_run1(s)));
            private static PayloadInt _run1(GcdStart s) {
                s.ContractedNotNull(nameof(s));
                while (s.A != s.B) {
                    s = s.A > s.B ? new GcdStart(s.A - s.B,    s.A   )
                      : s.A < s.B ? new GcdStart(   s.A,    s.B - s.A)
                                  : s;
                }
                return Payload.New(s, s.A);
            }

            // ~ 0.60 sec
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            [Description("Fully imperative; w/ substitution.")]
            public static readonly StateRes Run2 = GetResult(new StateInt(s=>_run2(s)));
            private static PayloadInt _run2(GcdStart s) {
                s.ContractedNotNull(nameof(s));
                while (s.A != s.B) {
                    var x = s.A; var y = s.B;       // explicitly exposes the immutability of s.
                    s = x > y ? new GcdStart(x-y,  x )
                      : x < y ? new GcdStart( x,  y-x)
                              : s;
                }
                return Payload.New(s, s.A);
            }
        }

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
            private static readonly StateBool GcdBody = new StateBool(s => Payload.New(s, s.A != s.B));
            private static readonly StateInt _run1 =
                GetCompose<GcdStart>(s =>
                              s.A > s.B ? Put(new GcdStart(s.A - s.B, s.B))
                            : s.A < s.B ? Put(new GcdStart(s.A, s.B - s.A))
                                        : Put(s)).Then(GcdBody)
                                                 .DoWhile().Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Straight Haskel (iterative).")]
            public static readonly StateRes Run1 = GetResult(_run1);

              // ~ 2.6 sec
            private static readonly StateInt _run2 = AlgorithmTransform.Modify().Then(GcdBody)
                                                                       .DoWhile().Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Straight Haskel w/ Modify() instead of Get.Compose(s => Put(transform(s))).")]
            public static readonly StateRes Run2 = GetResult(_run2);
        #endregion

              // ~ 1.5 sec
            private static readonly StateInt _run3 = AlgorithmState.DoWhile().Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Straight Haskel w/ DoWhile().")]
            public static readonly StateRes Run3 = GetResult(_run3);
        }

        /// <summary>TODO</summary>
        [Pure]
        internal static class Linq {
        #region Older LINQ implementations
              // ~ 3.5 sec
            private static readonly StateInt _run1 = new StateInt(start =>
                          ( from s in AlgorithmTransform.Enumerate(start)
                            let A = s.A
                            let B = s.B
                            where A == B
                            select Payload.New(s, A)
                          ).First());
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Basic LINQ w/ Enumerate(Transform<TState>, TState) and using l'let'.")]
            public static readonly StateRes Run1 = GetResult(_run1);

              // ~ 2.3 sec
            private static readonly StateInt _run2 = new StateBool(start =>
                          ( from payload in AlgorithmState.Enumerate(start)
                            where !payload.Value
                            select payload
                          ).First()).Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Better LINQ w/ Enumerate(State<TState,TValue>, TState).")]
            public static readonly StateRes Run2 = GetResult(_run2); 
        #endregion

               // ~ 1.5 sec
            private static readonly StateInt _run3 = new StateInt(start =>
                          ( from s in AlgorithmTransform.Enumerate(start)
                            where s.A == s.B
                            select Payload.New(s, s.A)
                          ).First());
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [Description("Best LINQ w/ Enumerate(Transform<TState>, TState) and w/o using 'let'.")]
            public static readonly StateRes Run3 = GetResult(_run3);
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [Pure]
        internal static class Best {
              // ~ 1.0 sec
            private static readonly StateInt _run = AlgorithmTransform.DoWhile(s => s.A != s.B)
                                                                      .Then(GcdExtract);
            /// <summary>TODO</summary>
            [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            [Description("Optimized DoWhile().")]
            public static readonly StateRes Run = GetResult(_run);
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly StateRes BestRun = s => Best.Run(s);
    }
}
