using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

namespace PGSolutions.Monads {
    [ExcludeFromCodeCoverage]
    public static class IOTests {
        [Fact]
        public static void IOTest() {
            bool isExecuted1 = false,
                 isExecuted2 = false,
                 isExecuted3 = false,
                 isExecuted4 = false;
            IO<int> one                       = () => { isExecuted1 = true; return 1; };
            IO<int> two                       = () => { isExecuted2 = true; return 2; };
            Func<int, IO<int>> addOne         = x  => { isExecuted3 = true; return (x + 1).ToIO(); };
            Func<int, Func<int, IO<int>>> add = x  => y => { isExecuted4 = true; return (x + y).ToIO(); };

            var query1 = ( from x in one
                           from y in two
                           from z in addOne(y)
                           from _ in "abc".ToIO()
                           let addOne2 = add(x)
                           select addOne2(z)
                         );
            Assert.False(isExecuted1 || isExecuted2 || isExecuted3 || isExecuted4); // Laziness.

            int lhs = 1 + 2 + 1;
            int rhs = query1.Invoke().Invoke();
            Assert.Equal(lhs, rhs); // Execution.

            Assert.True(isExecuted1 && isExecuted2 && isExecuted3 && isExecuted4);
        }

        [Fact]
        public static void IOTestFunctional() {
            bool isExecuted1 = false, isExecuted2 = false, isExecuted3 = false,
                 isExecuted4 = false, isExecuted5 = false, isExecuted6 = false;
            IO<int> one                       = () => { isExecuted1 = true; return 1; };
            IO<int> two                       = () => { isExecuted2 = true; return 2; };
            Func<int, IO<int>> addOne         = x  => { isExecuted3 = true; return (x + 1).ToIO(); };
            Func<int, Func<int, IO<int>>> add = x  => y => { isExecuted4 = true; return (x + y).ToIO(); };

            var query1 = one.SelectMany(x => two,          (x,y) => new { x, y })
                            .SelectMany(X => addOne(X.y),  (X,z) => new { X.x, X.y, z })
                            .SelectMany(X => "abc".ToIO(), (X,_) => new { X.x, X.y, X.z, _ })
                            .SelectMany(X => add(X.x)(X.z),(X,_) => new { X.x,X.y,X.z,X._, addOne2 = _ })
                            .Select(X => X.addOne2)
                            ;

            Func<int,IO<int>> Times2 = a => { isExecuted5=true; return () => 2 * a; };
            Func<int,IO<int>> Minus1 = a => { isExecuted6=true; return () => a - 1; };

            var query = query1.SelectMany(Times2).SelectMany(Minus1);

            Assert.False(isExecuted1 || isExecuted2 || isExecuted3
                      || isExecuted4 || isExecuted5 || isExecuted6); // Laziness.

            int lhs = 2 * (1 + 2 + 1) - 1;
            int rhs = query();
            Assert.Equal(lhs, rhs);

            Assert.True(isExecuted1 && isExecuted2 && isExecuted3
                     && isExecuted4 && isExecuted5 && isExecuted6); // Delayed execution.
        }

        static Func<int, IO<int>> addOne3 = x => (x + 1).ToIO();
        static IO<int> M = 1.ToIO();

        [Fact]
        public static void MonadLaw1Test() {
            // Monad law 1: m.Monad().SelectMany(f) == f(m)
            var lhs = 1.ToIO().SelectMany(addOne3);
            var rhs = addOne3(1);
            Assert.NotNull(lhs);
            Assert.NotNull(rhs);
            Assert.Equal(lhs(), rhs());
        }

        [Fact]
        public static void MonadLaw2Test() {
            // Monad law 2: M.SelectMany(Monad) == M
            var lhs = M.SelectMany(m => m.ToIO());
            var rhs = M;
            Assert.Equal(lhs(), rhs());
        }

        [Fact]
        public static void MonadLaw3Test() {
            // Monad law 3: M.SelectMany(f1).SelectMany(f2) == M.SelectMany(x => f1(x).SelectMany(f2))
            Func<int, IO<int>> addTwo = x => (x + 2).ToIO();
            var lhs1 = M.SelectMany(addOne3).SelectMany(addTwo);
            var rhs1 = M.SelectMany(x => addOne3(x).SelectMany(addTwo));
            Assert.Equal(lhs1(), rhs1());

            bool isExecuted5 = false, isExecuted6 = false, isExecuted7 = false;
            Func<int, IO<int>> addOne4          = x => { isExecuted5 = true; return (x + 1).ToIO(); };
            Func<string, IO<int>> length        = x => { isExecuted6 = true; return (x.Length).ToIO(); };
            Func<int, Func<int, IO<string>>> f7 = x => y => {
                isExecuted7 = true;
                return (new string('a', x + y)).ToIO();
            };

            Func<int, Func<string, IO<string>>> query2 = a => b => ( from x in addOne4(a).ToIO().Invoke()
                                                                     from y in length(b).ToIO().Invoke()
                                                                     from z in 0.ToIO()
                                                                     select f7 (x) (y)            
                                                                   ).Invoke();

            Assert.False(isExecuted5 || isExecuted6 || isExecuted7); // Laziness.

            var lhs = new string('a', 1 + 1 + "abc".Length);
            var rhs_  = query2(1);  Assert.NotNull(rhs_);
            var rhs = rhs_("abc");  Assert.NotNull(rhs);
            Assert.Equal(lhs, rhs()); // Execution.

            Assert.True(isExecuted5 && isExecuted6 && isExecuted7); // Deferred execution
        }
    }
}
