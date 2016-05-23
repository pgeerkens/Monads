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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;
using MsTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGSolutions.Utilities.Monads.UnitTests {
    //[MsTest.TestClass]
    //public class CommonTests {
    //  static IList<Maybe<String>> dataMaybe = new List<Maybe<String>>()
    //                              { "Fred", "George", null, "Ron", "Ginny" }.AsReadOnly();
    //  static IList<MaybeX<string>> dataMaybeX = new List<MaybeX<string>>()
    //                              { "Fred", "George", null, "Ron", "Ginny" }.AsReadOnly();

    //  [Theory][MsTest.TestMethod]
    //  [InlineData(dataMaybe)]
    //  [InlineData(dataMaybeX)]
    //  public void BasicTest1(dynamic data) {
    //      var dataMaybe   = data as  IList<Maybe<String>>;
    //      var dateMaybeX  = data as IList<MaybeX<string>>;

    //      string actual = "";
    //      if (dataMaybe != null)
    //          actual = string.Join("/", from e in dataMaybe
    //                                    select e.ToString()
    //                              ) + "/";
    //      else if (dataMaybeX != null)
    //          actual = string.Join("/", from e in dataMaybeX
    //                                    select e.ToString()
    //                              ) + "/";
    //      //else Xunit.Assert.Equal(BasicTest1,2);

    //      Contract.Assert(actual != null);
    //      Xunit.Assert.Equal("Fred/George//Ron/Ginny/", actual);
    //  }
    //}

    [MsTest.TestClass] [ExcludeFromCodeCoverage]
    public class ExecutionTests {

        static IList<Maybe<String>> data = new List<Maybe<String>>()
                        { "Fred", "George", null, "Ron", "Ginny" }.AsReadOnly();

        [Fact][MsTest.TestMethod]
        public void BasicTest1() {
            var actual = string.Join("/", from e in data
                                          select e.ToString()
                                    ) + "/";
            Contract.Assert(actual != null);
            Assert.Equal("Fred/George//Ron/Ginny/", actual);
        }
        [Fact][MsTest.TestMethod]
        public void BasicTest2() {
            var actual = string.Join("/", from e in data
                                          select e.ToNothingString()
                                    ) + "/";
            Contract.Assert(actual != null);
            Assert.Equal("Fred/George/Nothing/Ron/Ginny/", actual);
        }
        /// <summary>Equivalent code in first "Fluent" and then "Comprehension" syntax:</summary>
        [Fact][MsTest.TestMethod]
        public void SimpleLinqEquivalenceTest() {
            var expected = string.Join("/", data.Where(e => e.HasValue).
                                            Select(e => e.ToString())
                                    ) + "/";
            var actual = string.Join("/", from e in data
                                          where e.HasValue
                                          select e.ToString()
                                    ) + "/";
            //Contract.Assert(expected != null);  // redundant
            Contract.Assert(actual != null);
            Assert.Equal(expected, actual);
        }

        [Fact]
        [MsTest.TestMethod]
        public void ValueEqualityTest1() {
            string george = string.Copy("George");
            Assert.Equal(george, "George");
        }
        [Fact]
        [MsTest.TestMethod]
        public void ValueEqualityTest2() {
            string george = string.Copy("George");
            Assert.Equal("George/",
                    string.Join("/", from e in data
                                     let s = from item in e select item
                                     where s == george
                                     select e.ToNothingString()
                                ) + "/");
        }
        [Fact]
        [MsTest.TestMethod]
        public void ValueEqualityTest3() {
            string george = string.Copy("George");
            Assert.Equal("Fred/Nothing/Ron/Ginny/",
                string.Join("/", from e in data
                                 let s = from item in e select item
                                 where s != george
                                 select e.ToNothingString()
                            ) + "/");
        }

        [Fact]
        [MsTest.TestMethod]
        public void IncludedMiddleTest1() {
            Assert.Equal("George/",
                    string.Join("/", from e in data
                                     where e == "George"
                                     select e.ToNothingString()
                               ) + "/");
        }
        [Fact]
        [MsTest.TestMethod]
        public void IncludedMiddleTest2() {
            Assert.Equal("Fred/Nothing/Ron/Ginny/",
                    string.Join("/", from e in data
                                     where e != "George"
                                     select e.ToNothingString()
                               ) + "/");
        }

        [Fact]
        [MsTest.TestMethod]
        public void ExcludedMiddleTest1() {
            var expected = "George/";
            var actual = string.Join("/", from e in data
                                          where e.AreNonNullEqual("George") ?? false
                                          select e.ToNothingString()
                               ) + "/";
            Assert.Equal(expected, actual);
        }
        [Fact]
        [MsTest.TestMethod]
        public void ExcludedMiddleTest2() {
            Assert.Equal("Fred/Ron/Ginny/",
                    string.Join("/", from e in data
                                     where e.AreNonNullUnequal("George") ?? false
                                     select e.ToNothingString()
                               ) + "/");
        }
        [Fact]
        [MsTest.TestMethod]
        public void ExcludedMiddleTest3() {
            Assert.Equal("Nothing/",
                    string.Join("/", from e in data
                                     where !e.AreNonNullUnequal("George").HasValue
                                     select e.ToNothingString()
                                ) + "/");
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest1() {
            Assert.Equal("12",
                    (from x in 5.ToMaybe()
                     from y in 7.ToMaybe()
                     select x + y).ToNothingString());
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest2() {
            var expected = "Nothing";
            var actual = (from x in 5.ToMaybe()
                          from y in Maybe<int>.Nothing
                          select x + y
                    ).ToNothingString();
            Assert.Equal(expected, actual);
        }

        /// <summary>Chaining with LINQ Fluent syntax: one invalid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest3() {
            Assert.Equal("Nothing",
                5.ToMaybe().SelectMany(x => Maybe<int>.Nothing, (x, y) => new { x, y })
                           .Select(z => z.x + z.y)
                           .ToNothingString());
        }

        /// <summary>Equivalency of chaining in "Fluent" and then "Comprehension" syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void CompoundLinqEquivalenceTest() {
            Assert.Equal(
                5.ToMaybe().SelectMany(x => 7.ToMaybe(), (x, y) => new { x, y })
                           .Select(z => z.x + z.y)
                           .ToNothingString(),
                (from x in 5.ToMaybe()
                 from y in 7.ToMaybe()
                 select x + y
                ).ToNothingString());
        }
    }

    /// <remarks>
    /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-maybe.html
    /// </remarks>
    [MsTest.TestClass] [ExcludeFromCodeCoverage]
    public class MikeHadlowTests {
        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest1() {
            const int denominator = 2;
            var dt = DateTime.Now;
            Xunit.Assert.Equal("Hello World! 3 " + dt.ToShortDateString(),
                (from a in "Hello World!".ToMaybe()
                 from b in 12.DoSomeDivision(denominator, 2)
                 from c in dt.ToMaybe()
                 let sds = c.ToShortDateString()
                 select a + " " + b.ToString() + " " + sds
                ).ToNothingString());
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest2() {
            const int denominator = 0;

            Xunit.Assert.Equal("Nothing",
                (from a in "Hello World!".ToMaybe()
                 from b in 12.DoSomeDivision(denominator, 2)
                 from c in DateTime.Now.ToMaybe()
                 let sds = c.ToShortDateString()
                 select a + " " + b + " " + sds
                ).ToNothingString());
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest3() {
            var dt = DateTime.Now;
            Xunit.Assert.Equal("Hello World! 4 " + dt.ToShortDateString(),
                (from a in "Hello World!".ToMaybe()
                 from b in 24.DoSomeDivision(2, 3)
                 from c in dt.ToMaybe()
                 let sds = c.ToShortDateString()
                 select a + " " + b + " " + sds
                ).ToNothingString());
        }
    }

    [MsTest.TestClass] [ExcludeFromCodeCoverage]
    public class LazyTests {
        [Fact][MsTest.TestMethod]
        public void LazyTest() {
            var state = new ExternalState();
            var x = (from a in (Maybe<Func<int>>)state.GetState
                     select a
                    ).Extract();
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Xunit.Assert.Equal(0, y);

            Xunit.Assert.Equal(6, state.GetState());

            Xunit.Assert.Equal(7, x());

            //var xx = new Maybe<IList<int>>(null);
            //Console.WriteLine(xx);
        }

        private class ExternalState {
            private int _state;

            public ExternalState() { _state = -1; }
            public int GetState() { return ++_state; }
        }
    }

    [MsTest.TestClass]  [ExcludeFromCodeCoverage]
    public class MonadLawTests {
        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw1() {
            const string description = "Monad law 1: m.Monad().Bind(f) == f(m)";

            Func<int, Maybe<int>> addOne = x => x + 1;
            var lhs = 1.ToMaybe().SelectMany(addOne);
            var rhs = addOne(1);
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw2() {
            const string description = "Monad law 2: M.Bind(Monad) == M";

            var M = 4.ToMaybe();
            var lhs = M.SelectMany(i => i.ToMaybe());
            var rhs = M;
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw3() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            Func<int, Maybe<int>> addOne = x => x + 1;
            Func<int, Maybe<int>> addEight = x => x + 8;
            var M = 4.ToMaybe();
            var lhs = M.SelectMany(addOne).SelectMany(addEight);
            var rhs = M.SelectMany(x => addOne(x).SelectMany(addEight));
            Assert.True(lhs == rhs, description);
        }
    }
}
