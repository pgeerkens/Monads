using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;
using MsTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGSolutions.Utilities.Monads.UnitTests {
  [MsTest.TestClass()]
  public class IOTests {
    [MsTest.TestMethod()]
    public void IOTest() {
      bool isExecuted1 = false;
      bool isExecuted2 = false;
      bool isExecuted3 = false;
      bool isExecuted4 = false;
      IO<int> one = () => { isExecuted1 = true; return 1; }; Contract.Assert(one != null);
      IO<int> two = () => { isExecuted2 = true; return 2; }; Contract.Assert(two != null);
      Func<int, IO<int>> addOne = x => { isExecuted3 = true; return (x + 1).ToIO(); };
      Func<int, Func<int, IO<int>>> add = x => y => { isExecuted4 = true; return (x + y).ToIO(); };
      IO<IO<int>> query1 = from x in one
                           from y in two
                           from z in addOne(y)  //from z in addOne.Partial(y)()
                           from _ in "abc".ToIO()
                           let addOne2 = add(x)
                           select addOne2(z);
      Assert.False(isExecuted1); // Laziness.
      Assert.False(isExecuted2); // Laziness.
      Assert.False(isExecuted3); // Laziness.
      Assert.False(isExecuted4); // Laziness.

      var lhs = 1 + 2 + 1;
      Contract.Assert(query1 != null); var rhs_ = query1();
      Contract.Assume(rhs_   != null); var rhs = rhs_();
      Assert.Equal(lhs, rhs); // Execution.

      Assert.True(isExecuted1);
      Assert.True(isExecuted2);
      Assert.True(isExecuted3);
      Assert.True(isExecuted4);
    }

   
    static Func<int, IO<int>> addOne3 = x => (x + 1).ToIO();
    static IO<int> M = 1.ToIO();

    [MsTest.TestMethod()]
    public void MonadLaw1Test() {
      // Monad law 1: m.Monad().SelectMany(f) == f(m)
      IO<int> left = 1.ToIO().SelectMany(addOne3);Contract.Assert(left  != null);
      IO<int> right = addOne3(1);               Contract.Assume(right != null);
      var lhs = left();
      var rhs = right();
      Assert.Equal(lhs, rhs);
    }

    [MsTest.TestMethod()]
    public void MonadLaw2Test() {
      // Monad law 2: M.SelectMany(Monad) == M
      var left = M.SelectMany(m => m.ToIO());
      var right = M;
      Assert.Equal(left(), right());
    }

    [MsTest.TestMethod()]
    public void MonadLaw3Test() {
      // Monad law 3: M.SelectMany(f1).SelectMany(f2) == M.SelectMany(x => f1(x).SelectMany(f2))
      Func<int, IO<int>> addTwo = x => (x + 2).ToIO();
      var left = M.SelectMany(addOne3).SelectMany(addTwo);
      var right = M.SelectMany(x => addOne3(x).SelectMany(addTwo));
      Assert.Equal(left(), right());

      bool isExecuted5 = false;
      bool isExecuted6 = false;
      bool isExecuted7 = false;
      Func<int, IO<int>> addOne4 = x => { isExecuted5 = true; return (x + 1).ToIO(); };
      Func<string, IO<int>> length = x => { isExecuted6 = true; return (x.Length).ToIO(); };
      Func<int, Func<int, IO<string>>> f7 = x => y => 
            {Contract.Ensures(Contract.Result<IO<string>>() != null); isExecuted7 = true; return (new string('a', x + y)).ToIO(); };
      Func<int, Func<string, IO<string>>> query2 = a => b => ( from x in addOne4(a).ToIO()
                                                               from y in length(b).ToIO()
                                                               from z in 0.ToIO()
                                                               select f7 (x()) (y())
                                                             ) ();

      Assert.False(isExecuted5); // Laziness.
      Assert.False(isExecuted6); // Laziness.
      Assert.False(isExecuted7); // Laziness.

      var lhs = new string('a', 1 + 1 + "abc".Length);
      Contract.Assert(query2 != null);
      var rhs_  = query2(1);     Contract.Assume(rhs_  != null);
      var rhs_1 = rhs_("abc");   Contract.Assume(rhs_1 != null);
      var rhs =  rhs_1();        Contract.Assume(rhs   != null);
      Assert.Equal(lhs,rhs); // Execution.

      Assert.True(isExecuted5);
      Assert.True(isExecuted6);
      Assert.True(isExecuted7);
    }
  }
}
