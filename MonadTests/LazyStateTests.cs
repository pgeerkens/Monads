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

namespace PGSolutions.Monads {
    using static LazyState;
    using static FormattableString;
    using static CultureInfo;

    using LazyStateInt  = LazyState<string,int>;

    /// <summary>Unit tests for <see cref="LazyState{TState,TValue}"/></summary>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class LazyStateTests {
        public LazyStateTests() { }

        readonly static Func<int,int>          _f  = x => x * 2;
        readonly static Func<int,int>          _g  = x => x + 3;
        readonly static Func<int,int>          _h  = x => 4 * (x - 1);
        static readonly Func<int,LazyStateInt> _fm = i => _f(i).ToLazyState();
        static readonly Func<int,LazyStateInt> _gm = i => _g(i).ToLazyState();

        #region Functor Laws
        /// <summary>Functor Law #1: fmap id ≡ id.</summary>
        [Theory()]
        [InlineData("Start", 3)]
        public static void FunctorLaw1(string state, int v) {
            var lhs = v.ToLazyState().AsX();
            var rhs = from i in v.ToLazyState() select i;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply(state), rhs.Apply(state));
        }

        /// <summary>Functor Law #2: fmap (f . g) ≡ (fmap f) . (fmap g).</summary>
        [Theory()]
        [InlineData("Start", 3)]
        public static void FunctorLaw2(string state, int v) {
            var lhs = from s in v.ToLazyState() select _f(_g(s));
            var rhs = from s in ( from s in v.ToLazyState() select _g(s) ) select _f(s);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply(state), rhs.Apply(state));
        }
        #endregion
        #region Join Laws
        /// <summary>Return Law: return . f ≡ fmap f . return.</summary>
        /// <remarks>In expanded form: \x -> return (f x) = \x -> fmap f (return x).</remarks>
        [Theory()]
        [InlineData("Start", 3)]
        public static void ReturnLaw(string state, int v) {
            var lhs = ( _g(v).ToLazyState<string,int>() )?.Invoke(state);
            var rhs = ( (from s in v.ToLazyState() select _g(s)) | null)?.Invoke(state);

            Assert.True(lhs!=null);
            Assert.True(rhs!=null);
            Assert.Equal(lhs, rhs);
        }

        /// <summary>Join Law #1: ( join . fmap join ) ≡ ( join . join ).</summary>
        [Theory()]
        [InlineData("First",   1)]
        [InlineData("Second",  5)]
        [InlineData("Third",  16)]
        public static void JoinLaw1(string state, int v) {
            Func<int,LazyStateInt> f = i => new LazyStateInt(s => StatePayload.New(s+"-End", _f(i)) );

            var actual   = v.ToLazyState().SelectMany(f);
            var expected = f(v).AsX();

            Assert.True(expected.HasValue & actual.HasValue);
            Assert.Equal(expected.Apply(state), actual.Apply(state));
        }

        /// <summary>Join Law #2: join . fmap return  ≡  join . return  =  id.</summary>
        /// <remarks>In expanded form: \x -> join (fmap (return x) ) = \x -> join ( return x).</remarks>
        [Theory]
        [InlineData("First",   1)]
        [InlineData("Second",  5)]
        [InlineData("Third",  16)]
        public static void JoinLaw2(string state, int v) {
            Func<int,LazyStateInt> f = i => new LazyStateInt(s => StatePayload.New(s+"-End", _f(i)) );

            var actual   = f(v).SelectMany(i => i.ToLazyState());
            var expected = f(v).AsX();

            Assert.True(expected.HasValue & actual.HasValue);
            Assert.Equal(expected.Apply(state), actual.Apply(state));
            Assert.Equal(expected.Apply(state).Value.Value, _f(v));
        }

        /// <summary>Join Law #3: ( join . fmap (fmap f) ) ≡ ( fmap f . join ).</summary>
        /// <remarks>In expanded form: \x -> join (fmap (fmap f) x) = \x -> fmap f (join x).</remarks>
        [Theory()]
        [InlineData("First",   1)]
        [InlineData("Second",  5)]
        [InlineData("Third",  16)]
        public static void JoinLaw3(string state, int v) {
            Func<int,LazyStateInt> f = i => new LazyStateInt(s => StatePayload.New(s+nameof(_f), _f(i)) );
            Func<int,LazyStateInt> g = i => new LazyStateInt(s => StatePayload.New(s+nameof(_g), _g(i)) );
            Func<int,LazyStateInt> h = i => new LazyStateInt(s => StatePayload.New(s+nameof(_h), _h(i)) );

            var actual   = f(v).SelectMany(g).SelectMany(h);
            var expected = f(v).SelectMany(a => g(a).SelectMany(h)|null);

            Assert.True(expected.HasValue & actual.HasValue);
            Assert.Equal(expected.Apply(state), actual.Apply(state));

            actual   = from a in f(v) from b in g(a) from c in h(b) select c;
            expected = from c in (from a in f(v) from b in g(a) select b) from d in h(c) select d;

            Assert.True(expected.HasValue & actual.HasValue);
            Assert.Equal(expected.Apply(state), actual.Apply(state));
        }
        #endregion
        #region Monad Laws
        /// <summary>Monad Law #1: (return x).Bind(f) == f(x)</summary>
        [Theory]
        [InlineData("First", 3)]
        public static void MonadLaw1(string state, int v) {
            var lhs = _fm(v);
            var rhs = v.ToLazyState().SelectMany(_fm);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs(state), rhs.Apply(state));
        }

        /// <summary>Monad Law #2: M.Bind(Monad) == M</summary>
        [Theory]
        [InlineData("First", 3)]
        public static void MonadLaw2(string state, int v) {
            var lhs = v.ToLazyState().AsX();
            var rhs = v.ToLazyState().SelectMany(LazyState.ToLazyState);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply(state), rhs.Apply(state));
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Theory]
        [InlineData("First", 3)]
        public static void MonadLaw3A(string start, int v) {
            var lhs = from x in v.ToLazyState() from y in _fm(x) from r in _gm(y) select r;
            var rhs = from y in (from x in v.ToLazyState() from r in _fm(x) select r ) from r in _gm(y) select r;

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply(start), rhs.Apply(start));
        }

        /// <summary>Monad Law #3: (m >>= f) >>= g == m >>= ( \x -> (f x >>= g) ).</summary>
        [Theory]
        [InlineData("First", 3)]
        public static void MonadLaw3B(string start, int v) {
            var lhs = v.ToLazyState().SelectMany(_fm).SelectMany(_gm);
            var rhs = v.ToLazyState().SelectMany(x => _fm(x).SelectMany(_gm) | null);

            Assert.True(rhs.HasValue);
            Assert.Equal(lhs.Apply(start), rhs.Apply(start));
        }
        #endregion

        [Theory]
        [InlineData("First",   1,  4)]
        [InlineData("Second",  5,  3)]
        [InlineData("Third",  16,  7)]
        public static void LazyEvaluationTestSelectMany(string start, int p1, int p2) {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            Func<LazyStateInt> f1 = () => p1.ToLazyState<string,int>(
                            state => { isExecuted1 = true; return (state + "-a"); });
            Func<int, Func<int, Func<string, int>>> f2 =
                            x => y => z => { isExecuted2 = true; return x + y + z.Length; };
            var query = ( from f in f1().AsX()
                          from _ in Put(f.ToString(InvariantCulture))
                          from a in p2.ToLazyState<string, int>(state => Invariant($"{start}-b{state}"))
                          from b in Get<string>()
                          select f2(f)(a)(b)
                        );

            Assert.False(isExecuted1 || isExecuted2);   // Deferred and lazy.

            var expected = StatePayload.New(Invariant($"{start}-b{p1}"), (p1 + p2 + Invariant($"{start}-b{p1}").Length));
            var received = query.Apply(start);        // Execution.
            Assert.Equal(expected,received);

            Assert.True(isExecuted1 && isExecuted2);
        }
    }
}
