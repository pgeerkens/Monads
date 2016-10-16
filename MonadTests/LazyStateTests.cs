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

        readonly static LazyState<string,int>           _monad   = 1.LazyState<string,int>();
        readonly static Func<int,int>                   _addOneX = x => (x + 1);
        readonly static Func<int,LazyState<string,int>> _addOne  = x => (x + 1).LazyState<string,int>();
        readonly static Func<int,int,int>               _second  = (u,v) => v;

        [Fact]
        public static void LazyEvaluationTestSelectMany() {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            Func<LazyState<string,int>> f1 = () => 1.LazyState<string,int>(
                    state => { isExecuted1 = true; return (state + "a"); });
            Func<int, Func<int, Func<string, int>>> f2 =
                    x => y => z => { isExecuted2 = true; return x + y + z.Length; };
            var query2 = ( from ff in f1().AsX()
                           select from f in ff
                                  from _ in Put(f.ToString(InvariantCulture))
                                  from a in 2.LazyState<string, int>(state => "b" + state)
                                  from b in Get<string>()
                                  select f2(f)(a)(b)
                         ) | null;
            //var query3 = ( from f in f1().AsX()
            //               from _ in Put(string.Format("{0}",InvariantCulture,f)).AsX()
            //               from a in 2.LazyState<string, int>(state => "b" + state).AsX()
            //               from b in Get<string>().AsX()
            //               select f2(f)(a)(b)
            //             ) | null;
            //var query4 = ( f1().AsX().SelectMany(f=>Put(string.Format("{0}",InvariantCulture,f)).AsX(),   (f,_) => new { f,  _ })
            //                         .SelectMany(a=>2.LazyState<string, int>(state => "b" + state).AsX(), (z,a) => new {z.f, a })
            //                         .SelectMany(b=>Get<string>().AsX(),                                  (z,b) => f2(z.f)(z.a)(b))
            //             ) | null;
            var query = query2;
            Assert.False(isExecuted1);                  // Deferred and lazy.
            Assert.False(isExecuted2);                  // Deferred and lazy.

            var expected = StatePayload.New("b1", (1 + 2 + ("b1").Length));
            var rceeived = query?.Invoke("state");      // Execution.
            Assert.Equal(expected,rceeived);

            Assert.True(isExecuted1);
            Assert.True(isExecuted2);
        }

        #region Select() Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1Select() {
            var received = (_monad.AsX().Select(_addOneX) | null)("Start");
//                received = ( from m in _monad.AsX() select m..Select(_addOneX)("Start") )
//                         | default(StatePayload<string,int>);
            var expected = _addOne(1)("Start");

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
            Func<int, LazyState<string,int>> addTwo = x => (x + 2).LazyState<string,int>();
            var received = ( _monad.Select(_addOneX).SelectMany(addTwo) | null )?.Invoke("Start");
            var expected = ( from x1 in _monad
                             from x2 in _addOne(x1)
                             from x3 in addTwo(x2)
                             select x3
                           ).Invoke("Start");
            Assert.True(received!=null);
            Assert.Equal(expected, received);
        }
        #endregion

        #region SelectMany() Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1SelectMany() {
            var received = ( _monad.AsX().SelectMany(_addOne) | null )?.Invoke("Start");
            var expected = _addOne(1)("Start");

            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2SelectMany() {
            var received = ( _monad.AsX().SelectMany(StateExtensions.LazyState<string,int>) | null )?.Invoke("Start");
            var expected = _monad("Start");

            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3SelectMany() {
            Func<int, LazyState<string,int>> addTwo = x => (x + 2).LazyState<string,int>();
            var received = ( _monad.AsX().SelectMany(_addOne).SelectMany(addTwo) | null )?.Invoke("Start");
            var expected = ( from x1 in _monad
                             from x2 in _addOne(x1)
                             from x3 in addTwo(x2)
                             select x3 ).Invoke("Start");
            Assert.True(received!=null);
            Assert.Equal(expected,received);
        }
        #endregion

        #region SelectMany(,) Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1SelectMany2() {
            var received = _monad.SelectMany(_addOne,_second)("Start");
            var expected = _addOne(1)("Start");

            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2SelectMany2() {
            var received = _monad.SelectMany(StateExtensions.LazyState<string,int>,_second)("Start");
            var expected = _monad("Start");

            Assert.Equal(expected,received);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3SelectMany2() {
            Func<int, LazyState<string,int>> addTwo = x => (x + 2).LazyState<string,int>();
            var received = _monad.SelectMany(_addOne,_second).SelectMany(addTwo,_second)("Start");
            var expected = _monad.SelectMany(x => _addOne(x).SelectMany(addTwo,_second),_second)("Start");

            Assert.Equal(expected,received);
        }
        #endregion
    }

    /// <summary>Extension methods for <see cref="StateTuple<TState,TValue>"/>.</summary>
    internal static partial class StateExtensions {
        /// <summary>η: T -> State&lt;T, TState&gt;</summary> 
        public static LazyState<TState,TValue> LazyState<TState, TValue>(this TValue value) =>
                state => StatePayload.New(state, value);

        /// <summary>η: T -> State&lt;T, TState&gt;</summary> 
        public static LazyState<TState, TValue> LazyState<TState, TValue>(this TValue value,
            Func<TState,TState> transform
        ) => oldState => StatePayload.New(transform(oldState), value);
    }
}
