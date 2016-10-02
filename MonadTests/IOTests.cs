using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using Xunit;

namespace PGSolutions.Monads.MonadTests {
    [ExcludeFromCodeCoverage]
    public static class IOTests {
        [Fact]
        public static void IOTest() {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            bool isExecuted3 = false;
            bool isExecuted4 = false;
            IO<int> one = () => { isExecuted1 = true; return 1; };
            IO<int> two = () => { isExecuted2 = true; return 2; };
            Func<int, IO<int>> addOne = x => { isExecuted3 = true; return (x + 1).ToIO(); };
            Func<int, Func<int, IO<int>>> add = x => y => { isExecuted4 = true; return (x + y).ToIO(); };

            var query1 = ( from x in one
                           from y in two
                           from z in addOne(y)
                           from _ in "abc".ToIO()
                           let addOne2 = add(x)
                           select addOne2(z)
                         );
            Assert.False(isExecuted1); // Laziness.
            Assert.False(isExecuted2); // Laziness.
            Assert.False(isExecuted3); // Laziness.
            Assert.False(isExecuted4); // Laziness.

            int lhs = 1 + 2 + 1;
            int rhs = query1.Invoke().Invoke();
            Assert.Equal(lhs, rhs); // Execution.

            Assert.True(isExecuted1);
            Assert.True(isExecuted2);
            Assert.True(isExecuted3);
            Assert.True(isExecuted4);
        }

        [Fact]
        public static void IOTestFunctional() {
            bool isExecuted1 = false;
            bool isExecuted2 = false;
            bool isExecuted3 = false;
            bool isExecuted4 = false;
            bool isExecuted5 = false;
            bool isExecuted6 = false;
            IO<int> one = () => { isExecuted1 = true; return 1; };
            IO<int> two = () => { isExecuted2 = true; return 2; };
            Func<int, IO<int>> addOne = x => { isExecuted3 = true; return (x + 1).ToIO(); };
            Func<int, Func<int, IO<int>>> add = x => y => { isExecuted4 = true; return (x + y).ToIO(); };

            var query1 = one.SelectMany(x => two,          (x,y) => new { x, y })
                            .SelectMany(X => addOne(X.y),  (X,z) => new { X.x, X.y, z })
                            .SelectMany(X => "abc".ToIO(), (X,_) => new { X.x, X.y, X.z, _ })
                            .SelectMany(X => add(X.x)(X.z),(X,_) => new { X.x,X.y,X.z,X._, addOne2 = _ })
                            .Select(X => X.addOne2)
                            ;

            Func<int,IO<int>> Times2 = a => { isExecuted5=true; return () => 2 * a; };
            Func<int,IO<int>> Minus1 = a => { isExecuted6=true; return () => a - 1; };

            var query = query1.SelectMany(Times2).SelectMany(Minus1);

            Assert.False(isExecuted1); // Laziness.
            Assert.False(isExecuted2); // Laziness.
            Assert.False(isExecuted3); // Laziness.
            Assert.False(isExecuted4); // Laziness.
            Assert.False(isExecuted5); // Laziness.
            Assert.False(isExecuted6); // Laziness.

            int lhs = 2 * (1 + 2 + 1) - 1;
            int rhs = query.Invoke();
            Assert.Equal(lhs, rhs);

            Assert.True(isExecuted1); // Delayed execution.
            Assert.True(isExecuted2); // Delayed execution.
            Assert.True(isExecuted3); // Delayed execution.
            Assert.True(isExecuted4); // Delayed execution.
            Assert.True(isExecuted5); // Delayed execution.
            Assert.True(isExecuted6); // Delayed execution.
        }

        static Func<int, IO<int>> addOne3 = x => (x + 1).ToIO();
        static IO<int> M = 1.ToIO();

        [Fact]
        public static void MonadLaw1Test() {
            // Monad law 1: m.Monad().SelectMany(f) == f(m)
            var lhs = 1.ToIO().SelectMany(addOne3); Contract.Assert(lhs  != null);
            var rhs = addOne3(1);                        //Contract.Assume(rhs != null);
            Assert.Equal(lhs.Invoke(), rhs.Invoke());
        }

        [Fact]
        public static void MonadLaw2Test() {
            // Monad law 2: M.SelectMany(Monad) == M
            var lhs = M.SelectMany(m => m.ToIO());
            var rhs = M;
            Assert.Equal(lhs.Invoke(), rhs.Invoke());
        }

        [Fact]
        public static void MonadLaw3Test() {
            // Monad law 3: M.SelectMany(f1).SelectMany(f2) == M.SelectMany(x => f1(x).SelectMany(f2))
            Func<int, IO<int>> addTwo = x => (x + 2).ToIO();
            var lhs1 = M.SelectMany(addOne3).SelectMany(addTwo);
            var rhs1 = M.SelectMany(x => addOne3(x).SelectMany(addTwo));
            Assert.Equal(lhs1.Invoke(), rhs1.Invoke());

            bool isExecuted5 = false;
            bool isExecuted6 = false;
            bool isExecuted7 = false;
            Func<int, IO<int>> addOne4 = x => { isExecuted5 = true; return (x + 1).ToIO(); };
            Func<string, IO<int>> length = x => { isExecuted6 = true; return (x.Length).ToIO(); };
            Func<int, Func<int, IO<string>>> f7 = x => y => {
                Contract.Ensures(Contract.Result<IO<string>>() != null);
                isExecuted7 = true;
                return (new string('a', x + y)).ToIO();
            };

            Func<int, Func<string, IO<string>>> query2 = a => b => ( from x in addOne4(a).ToIO().Invoke()
                                                                     from y in length(b).ToIO().Invoke()
                                                                     from z in 0.ToIO()
                                                                     select f7 (x) (y)            
                                                                   ).Invoke();

            Assert.False(isExecuted5); // Laziness.
            Assert.False(isExecuted6); // Laziness.
            Assert.False(isExecuted7); // Laziness.

            var lhs = new string('a', 1 + 1 + "abc".Length);
            Contract.Assert(query2 != null);
            var rhs_  = query2(1);      Contract.Assume(rhs_  != null);
            var rhs = rhs_("abc");
            Assert.Equal(lhs, rhs.Invoke()); // Execution.

            Assert.True(isExecuted5);
            Assert.True(isExecuted6);
            Assert.True(isExecuted7);
        }
    }
}
