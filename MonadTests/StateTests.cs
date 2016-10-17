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
using System.Diagnostics.Contracts;
using System.Globalization;

using Xunit;

namespace PGSolutions.Monads.MonadTests {
    using static State;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class StateTests {
        public StateTests() { }

        readonly static State<X<string>,int>           _monad = 1.State<X<string>,int>();
        readonly static Func<int,int>               _addOneX = x => (x + 1);
        readonly static Func<int,State<X<string>,int>> _addOne  = x => (x + 1).State<X<string>,int>();
        readonly static Func<int,int,int>           _second  = (u,v) => v;

        [Fact]
        public static void LazyEvaluationTestSelectMany() {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            Func<State<string,int>> f1 = () => 1.State<string,int>(
                    state => { isExecuted1 = true; return (state + "a"); });
            Func<int, Func<int, Func<string, int>>> f2 =
                    x => y => z => { isExecuted2 = true; return x + y + z.Length; };
            State<string, int> query1 = from x in f1()
                                        from _ in Put(x.ToString(CultureInfo.InvariantCulture))
                                        from y in 2.State<string, int>(state => "b" + state)
                                        from z in Get<string>()
                                        select f2(x)(y)(z);
            Assert.False(isExecuted1); // Deferred and lazy.
            Assert.False(isExecuted2); // Deferred and lazy.
            StructTuple<string,int> result1 = query1("state"); // Execution.
            Assert.True(result1.Value.Equals(1 + 2 + ("b" + "1").Length));
            Assert.True(result1.State.Equals("b" + "1"));
            Assert.True(isExecuted1);
            Assert.True(isExecuted2);
        }

        #region Select() Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1Select() {
            var received = _monad.Select(_addOneX);
            var expected = _addOne(1);
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2Select() {
            var received = _monad.Select(u=>u);
            var expected = _monad;
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3Select() {
            Func<int,State<X<string>,int>> addTwo = x => (x + 2).State<X<string>,int>();
            var received = _monad.SelectMany(_addOne).Select(addTwo)("a");
            var expected = _monad.Select(x => _addOne(x).SelectMany(addTwo)(""))("a");

            var expectedState = expected.State;
            var receivedState = received.State;
            Assert.Equal(expectedState,receivedState);
            var expectedValue = expected.Value.Value;
            var receivedValue = received.Value("").Value;
            Assert.Equal(expectedValue,receivedValue);
        }
        #endregion

        #region SelectMany() Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1SelectMany() {
            var received = _monad.SelectMany(_addOne);
            var expected = _addOne(1);
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2SelectMany() {
            var received = _monad.SelectMany(StateExtensions.State<X<string>,int>);
            var expected = _monad;
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3SelectMany() {
            Func<int, State<X<string>,int>> addTwo = x => (x + 2).State<X<string>,int>();
            var received = _monad.SelectMany(_addOne).SelectMany(addTwo);
            var expected = _monad.SelectMany(x => _addOne(x).SelectMany(addTwo));
            Assert.Equal(expected.Value("a"), received.Value("a"));
            Assert.Equal(expected.State("a"), received.State("a"));
        }
        #endregion

        #region SelectMany(,) Monad Law Tests
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaw1SelectMany2() {
            var received = _monad.SelectMany(_addOne,_second);
            var expected = _addOne(1);
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2SelectMany2() {
            var received = _monad.SelectMany(StateExtensions.State<X<string>,int>,_second);
            var expected = _monad;
            Assert.Equal(received.Value("a"), expected.Value("a"));
            Assert.Equal(received.State("a"), expected.State("a"));
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public static void MonadLaw3SelectMany2() {
            Func<int,State<X<string>,int>> addTwo = x => (x + 2).State<X<string>,int>();
            var received = _monad.SelectMany(_addOne,_second).SelectMany(addTwo,_second);
            var expected = _monad.SelectMany(x => _addOne(x).SelectMany(addTwo,_second),_second);
            Assert.Equal(expected.Value("a"), received.Value("a"));
            Assert.Equal(expected.State("a"), received.State("a"));
        }
        #endregion
    }

    /// <summary>Extension methods for <see cref="StateTuple<TState,TValue>"/>.</summary>
    internal static partial class StateExtensions {
        /// <summary>η: T -> State{TState,TValue}</summary> 
        [Pure]
        public static State<TState, TValue> State<TState, TValue>(this TValue value) =>
                state => StructTuple.New(state, value);

        /// <summary>η: T -> State{TState,TValue}</summary> 
        [Pure]
        public static State<TState, TValue> State<TState, TValue>(this
            TValue value,
            Func<TState, TState> newState
        ) => oldState => StructTuple.New(newState(oldState), value);
    }
}
