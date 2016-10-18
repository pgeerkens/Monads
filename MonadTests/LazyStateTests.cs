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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Xunit;

namespace PGSolutions.Monads.MonadTests {
    using static LazyState;
    using static CultureInfo;

    using Maybe_T = LazyState<string,object>;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class LazyStateTests {
        public LazyStateTests() { }

        const           int                             _v  = 1;
        readonly static LazyState<string,int>           _m  = _v.ToMonad();
        readonly static Func<int,int>                   _f  = x => x * 2;
        readonly static Func<int,int>                   _g  = x => x + 3;
        static readonly Func<int,LazyState<string,int>> _fm = i => _f(i).ToMonad();
        static readonly Func<int,LazyState<string,int>> _gm = i => _g(i).ToMonad();

        #region Functor Laws
        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Fact]
        public static void FunctorLaw1() {
            var lhs = _m.AsX();
            var rhs = from i in _m select i;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Fact]
        public static void FunctorLaw2() {
            var lhs = from s in _m select _f(_g(s));
            var rhs = from s in ( from s in _m select _g(s) ) select _f(s);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }
        #endregion
        #region Join Laws
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        /// <remarks>In expanded form: \x -> return (f x) = \x -> fmap f (return x).</remarks>
        [Fact]
        public static void ReturnLaw() {
            var lhs = ( _g(_v).ToLazyState<string,int>() )?.Invoke("Start");
            var rhs = ( (from s in _m select _g(s)) | null)?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #1: ( join . fmap join ) ≡ ( join . join ).</summary>
        [Fact]
        public static void JoinLaw1() {
            var m   = ((object)((object)_m).ToMonad()).ToMonad();
            var lhs = from x1 in m
                      from x2 in x1.ToMonad()
                      from r  in (Maybe_T)x2
                      select r;
            var rhs = from x3 in ( from x1 in m from x2 in x1.ToMonad() select x2 )
                      from r in (Maybe_T)x3
                      select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }

        /// <summary>Join Law #2: ( join . fmap return ) ≡ ( join . return = id ).</summary>
        [Fact]
        public static void JoinLaw2() {
            var lhs = from x1 in _m from x2 in x1.ToMonad() select x2;
            var rhs = from x1 in _m from x2 in x1.ToMonad() select x2;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }

        /// <summary>Join Law #3: ( join . fmap (fmap f) ) ≡ ( fmap f . join ).</summary>
        /// <remarks>In expanded form: \x -> join (fmap (fmap f) x) = \x -> fmap f (join x).</remarks>
        [Fact]
        public static void JoinLaw3() {
            var lhs = from x1 in _m.Select(_f).Select(_g) select x1;
            var rhs = from x2 in ( from x1 in _m select _f(x1) ) select _g(x2);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }
        #endregion
        #region Monad Laws
        /// <summary>Monad Law #1: (return x).Bind(f) == f(x)</summary>
        [Fact]
        public static void MonadLaw1() {
            var lhs = _fm(_v);
            var rhs = _v.ToMonad().SelectMany(_fm);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Invoke("Start"), rhs.Apply("Start"));
        }

        /// <summary>Monad Law #2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2() {
            var lhs = _m.AsX();
            var rhs = _m.SelectMany(Extensions.ToMonad);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Fact]
        public static void MonadLaw3A() {
            var lhs = from x in _m from y in _fm(x) from r in _gm(y) select r;
            var rhs = from y in (from x in _m from r in _fm(x) select r ) from r in _gm(y) select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Fact]
        public static void MonadLaw3B() {
            var lhs = _m.SelectMany(_fm).SelectMany(_gm);
            var rhs = _m.SelectMany(x => _fm(x).SelectMany(_gm) | null);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply("Start"), rhs.Apply("Start"));
        }
        #endregion

        [Fact]
        public static void LazyEvaluationTestSelectMany() {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            Func<LazyState<string,int>> f1 = () => 1.ToMonad<string,int>(
                            state => { isExecuted1 = true; return (state + "a"); });
            Func<int, Func<int, Func<string, int>>> f2 =
                            x => y => z => { isExecuted2 = true; return x + y + z.Length; };
            var query = ( from f in f1().AsX()
                          from _ in Put(f.ToString(InvariantCulture))
                          from a in 2.ToMonad<string, int>(state => "b" + state)
                          from b in Get<string>()
                          select f2(f)(a)(b)
                        );

            Assert.False(isExecuted1);                  // Deferred and lazy.
            Assert.False(isExecuted2);                  // Deferred and lazy.

            var expected = StatePayload.New("b1", (1 + 2 + ("b1").Length));
            var rceeived = query.Apply("state");        // Execution.
            Assert.Equal(expected,rceeived);

            Assert.True(isExecuted1);
            Assert.True(isExecuted2);
        }
    }

    /// <summary>Extension methods for <see cref="LazyStateTests"/>.</summary>
    internal static partial class Extensions {
        /// <summary>η: T -> State{TState,TValue}</summary> 
        public static LazyState<string,TValue>      ToMonad<TValue>(this TValue value) => 
                state => StatePayload.New(state, value);

        /// <summary>η: T -> State{TState,TValue}</summary> 
        public static LazyState<TState,TValue>     ToMonad<TState,TValue>(this TValue value,
            Func<TState,TState> transform
        ) => oldState => StatePayload.New(transform(oldState), value);
    }
}
