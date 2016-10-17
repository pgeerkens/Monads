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

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class LazyStateTests {
        public LazyStateTests() { }

        const           int                     _value  = 1;
        readonly static LazyState<string,int>   _monad  = _value.ToMonad();
        readonly static Func<int,int>           _addOne = x => (x + 1);
        readonly static Func<int,int>           _times2 = x => x * 2;
        readonly static Func<int,int>           _plus3  = x => x + 3;

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
                        ) | null;

            Assert.False(isExecuted1);                  // Deferred and lazy.
            Assert.False(isExecuted2);                  // Deferred and lazy.

            var expected = StatePayload.New("b1", (1 + 2 + ("b1").Length));
            var rceeived = query?.Invoke("state");      // Execution.
            Assert.Equal(expected,rceeived);

            Assert.True(isExecuted1);
            Assert.True(isExecuted2);
        }

        #region FUnctor Laws
        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Fact]
        public static void FunctorLaw1() {
            var lhs = ( ( from i in _monad select i) | null)?.Invoke("Start");
            var rhs = _monad?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Fact]
        public static void FunctorLaw2() {
            var lhs = ( ( from s in _monad select _times2(_plus3(s))) | null)?.Invoke("Start");
            var rhs = ( ( from s in ( from s in _monad select _plus3(s) )
                          select _times2(s)
                        ) | null
                      )?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }
        #endregion
        #region Join Laws
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        /// <remarks>In expanded form: \x -> return (f x) = \x -> fmap f (return x).</remarks>
        [Fact]
        public static void ReturnLaw() {
            var lhs = ( _plus3(_value).ToLazyState<string,int>() )?.Invoke("Start");
            var rhs = ( (from s in _monad select _plus3(s)) | null)?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #1: ( join . fmap join ) ≡ ( join . join ).</summary>
        [Fact]
        public static void JoinLaw1() {
            var m = (_monad).ToLazyState<string,object>().ToLazyState<string,object>();
            var lhs = ( ( from x1 in m
                          from x2 in x1.ToLazyState<string,object>()
                          from r in (LazyState<string,object>)x2
                          select r as int?) ?? null
                      )?.Invoke("Start");
            var rhs = ( ( from x3 in ( from x1 in m from x2 in x1.ToLazyState<string,object>() select x2 )
                          from r in (LazyState<string,object>)x3
                          select r as int?
                        ) ?? null
                      )?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #2: ( join . fmap return ) ≡ ( join . return = id ).</summary>
        [Fact]
        public static void JoinLaw2() {
            var lhs = (from x1 in _monad from x2 in x1.ToLazyState<string,int>() select x2)?.Invoke("Start");
            var rhs = (from x1 in _monad from x2 in x1.ToLazyState<string,int>() select x2)?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #3: ( join . fmap (fmap f) ) ≡ ( fmap f . join ).</summary>
        /// <remarks>In expanded form: \x -> join (fmap (fmap f) x) = \x -> fmap f (join x).</remarks>
        [Fact]
        public static void JoinLaw3() {
            var lhs = ( (from x1 in _monad.Select(_times2).Select(_plus3) select x1) | null)?.Invoke("Start");
            var rhs = ( (from x2 in ( from x1 in _monad select _times2(x1) ) select _plus3(x2)) | null)?.Invoke("Start");

            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }
        #endregion

        #region SelectMany() Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1SelectMany() {
            var received = ( ( from m in _monad select _addOne(m) ) | null )?.Invoke("Start");
            var expected = _addOne(1).ToMonad()("Start");

            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2SelectMany() {
            var received = ( _monad.SelectMany(Extensions.ToMonad) | null )?.Invoke("Start");
            var expected = _monad?.Invoke("Start");

            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3SelectMany() {
            Func<int, LazyState<string,int>> addTwo = x => (x + 2).ToMonad();
            var received = ( _monad.SelectMany(i=>_addOne(i).ToMonad()).SelectMany(addTwo) | null )?.Invoke("Start");
            var expected = ( from x1 in _monad
                             from x2 in _addOne(x1).ToMonad()
                             from x3 in addTwo(x2)
                             select x3 ).Invoke("Start");
            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }
        #endregion

        #region Select() Monad Law Tests - possibly irrelevant now
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1Select() {
            var received = (_monad.Select(_addOne) | null)?.Invoke("Start");
            var expected = _addOne(1).ToMonad()?.Invoke("Start");

            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2Select() {
            var received = _monad.Select(u=>u).SelectMany(m => m?.Invoke("Start"));
            var expected = _monad("Start");

            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3Select() {
            Func<int, LazyState<string,int>> addTwo = x => (x + 2).ToMonad();
            var received = ( _monad.Select(_addOne).SelectMany(addTwo) | null )?.Invoke("Start");
            var expected = ( from x1 in _monad
                             from x2 in _addOne(x1).ToMonad()
                             from x3 in addTwo(x2)
                             select x3
                           ).Invoke("Start");
            Assert.True(received!=null);
            Assert.Equal(expected, received);
        }
        #endregion
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
