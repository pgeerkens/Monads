using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;

namespace PGSolutions.Monads.UnitTests {

    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors",
        Justification = "Unit tests require a public default constructor.")]
    [ExcludeFromCodeCoverage]
    public partial class EnumerableTests {
        [Fact]
        public static void ExecutionTest() {
            var expected = new List<int> { 2, 2, 3 };
            bool isExecuted1 = false;
            Func<int, Func<int, int>> f = x => y => { isExecuted1 = true; return x + y; };
            var query = from x in new int[] { 0, 1 }
                        from y in new int[] { 1, 2 }
                        let z = f(x)(y)
                        where z > 1
                        select z;
            Assert.False(isExecuted1, "isExecuted: laziness");

            var result = query.ToList();
            Assert.True(isExecuted1, "isExecuted: execution");

            Assert.Equal(expected, result);
        }

        static readonly Func<int, IEnumerable<int>> _plusOne  = x => (x + 1).Enumerable();
        static readonly Func<int, IEnumerable<int>> _timesTwo = x => (x * 2).Enumerable();
        static readonly IEnumerable<int> _enumerable1 = new int[] { 0, 1 };
        static readonly IEnumerable<int> _enumerable2 = new int[] { 2, 4 };

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLawsTest1() {
            var plusOne  = _plusOne(1);   Contract.Assume(plusOne != null);
            var expected = plusOne.ToList();
            var received = 1.Enumerable().SelectMany(_plusOne).ToList();
            Assert.NotNull(received);

            Assert.Equal(expected, received);
        }
        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLawsTest2() {
            var expected = _enumerable1.ToList();
            var received = _enumerable1.SelectMany(EnumerableExtensions.Enumerable).ToList();

            Assert.Equal(expected, received);
        }
        /// <summary>Monad law 3: M.Bind(f).Bind(g) == M.Bind(x => f(x).Bind(g))</summary>
        [Fact]
        public static void MonadLawsTest3() {
            var expected = _enumerable2.SelectMany(x => _plusOne(x).SelectMany(_timesTwo)).ToList();
            var received = _enumerable2.SelectMany(_plusOne).SelectMany(_timesTwo).ToList();

            Assert.Equal(expected, received);
        }
    }
}
