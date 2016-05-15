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

    [MsTest.TestClass] [ExcludeFromCodeCoverage]
    public class MaybeXTests {
        internal static IList<MaybeX<string>> data = new List<MaybeX<string>>()
                                { "Fred", "George", null, "Ron", "Ginny" }.AsReadOnly();

        [Fact][MsTest.TestMethod]
        public void BasicTest1() {
            Assert.Equal("Fred/George//Ron/Ginny/",
                    string.Join("/", from e in data
                                    select e.ToString()) + "/");
        }
        [Fact][MsTest.TestMethod]
        public void BasicTest2() {
            Assert.Equal("Fred/George/Nothing/Ron/Ginny/",
                    string.Join("/",  from e in data
                                        select e.ToNothingString() ) + "/" );

        }
        /// <summary>Equivalent code in first "Fluent" and then "Comprehension" syntax:</summary>
        [Fact][MsTest.TestMethod]
        public void SimpleLinqEquivalenceTest() {
            Assert.Equal(
                    string.Join("/", data.Where (e => e.HasValue).
                                            Select(e => e.ToString())
                                ) + "/",
                    string.Join("/",  from e in data
                                        where e.HasValue
                                        select e.ToString()
                                ) + "/" );
        }

        [Fact][MsTest.TestMethod]
        public void ValueEqualityTest1() {
            string george = string.Copy("George");
            Assert.Equal(george,"George");
        }
        [Fact][MsTest.TestMethod]
        public void ValueEqualityTest2() {
            string george = string.Copy("George");
            Assert.Equal("George/",
                    string.Join("/",  from e in data
                                        let s = from item in e select item
                                        where s == george
                                        select e.ToNothingString()
                                ) + "/" );
        }
        [Fact][MsTest.TestMethod]
        public void ValueEqualityTest3() {
            string george = string.Copy("George");
                Assert.Equal("Fred/Nothing/Ron/Ginny/",
                    string.Join("/",  from e in data
                                        let s = from item in e select item
                                        where s != george
                                        select e.ToNothingString()
                                ) + "/" );
        }

        [Fact][MsTest.TestMethod]
        public void IncludedMiddleTest1() {
            Assert.Equal("George/",
                    string.Join("/",  from e in data
                                      where e == "George"
                                      select e.ToNothingString()
                               ) + "/" );
        }
        [Fact][MsTest.TestMethod]
        public void IncludedMiddleTest2() {
            Assert.Equal("Fred/Nothing/Ron/Ginny/",
                    string.Join("/",  from e in data
                                      where e != "George"
                                      select e.ToNothingString()
                               ) + "/" );
        }

        [Fact][MsTest.TestMethod]
        public void ExcludedMiddleTest1() {
            Assert.Equal("George/",
                    string.Join("/",  from e in data
                                      where e.AreNonNullEqual("George") ?? false
                                      select e.ToNothingString()
                               ) + "/" );
        }
        [Fact][MsTest.TestMethod]
        public void ExcludedMiddleTest2() {
            Assert.Equal("Fred/Ron/Ginny/",
                    string.Join("/",  from e in data
                                      where e.AreNonNullUnequal("George") ?? false
                                      select e.ToNothingString()
                               ) + "/" );
        }
        [Fact][MsTest.TestMethod]
        public void ExcludedMiddleTest3() {
            Assert.Equal("Nothing/",
                    string.Join("/",  from e in data
                                      where ! e.AreNonNullUnequal("George").HasValue
                                      select e.ToNothingString()
                                ) + "/" );
        }

        [Fact][MsTest.TestMethod]
        public void MemberAccessTestNotNothing() {
            Assert.Equal("Fred/Ron/Ginny/",
                    string.Join("/",  from e in data
                                      where e.SelectMany<string>(s=>s).AreNonNullUnequal("George") ?? false
                                      select e.ToNothingString()
                                ) + "/" );
        }

        [Fact][MsTest.TestMethod]
        public void LazyTest() {
            var state = new ExternalState();
            var x = ( from a in new MaybeX<Func<int>>(state.GetState)
                    select a
                    ) | (()=>0);
            var y = x();

            for (int i = 0; i++ < 5; ) state.GetState();

            Assert.Equal(0, y);

            Assert.Equal(6, state.GetState());

            Assert.Equal(7, x());

            //var xx = new MaybeX<IList<int>>(null);
            //Console.WriteLine(xx);
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest1() {
            Assert.Equal("Fred Weasley",
                    ( from x in "Fred".ToMaybeX()
                        from y in " Weasley".ToMaybeX()
                        select x + y ).ToNothingString() );
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest2() {
            Assert.Equal("Nothing",
                    ( from x in "Fred".ToMaybeX()
                        from y in MaybeX<string>.Nothing
                        select x + y
                    ).ToNothingString() );
        }

        /// <summary>Chaining with LINQ Fluent syntax: one invalid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void WesDyerTest3() {
            Assert.Equal("Nothing",
                    "Fred".ToMaybeX().SelectMany(x => MaybeX<string>.Nothing, (x, y) => new { x, y })
                                     .Select(z => z.x + z.y)
                                     .ToNothingString() );
        }

        /// <summary>Equivalency of chaining in "Fluent" and then "Comprehension" syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void CompoundLinqEquivalenceTest() {
            Assert.Equal(
                    "Fred".ToMaybeX().SelectMany(x => " Weasley".ToMaybeX(), (x, y) => new { x, y })
                                     .Select(z => z.x + z.y )
                                     .ToNothingString(),
                    ( from x in "Fred".ToMaybeX()
                      from y in " Weasley".ToMaybeX()
                      select x + y
                    ).ToNothingString() );
        }

        static readonly Func<string,MaybeX<string>> addOne = x => x + "constant";
        static readonly Func<string,MaybeX<string>> addEight = x => x + "/" + x;

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw1MaybeX() {
            const string description = "Monad law 1: m.Monad().Bind(f) == f(m)";

            var lhs = "1".ToMaybeX().SelectMany(addOne);
            var rhs = addOne("1");
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw2MaybeX() {
            const string description = "Monad law 2: M.Bind(Monad) == M";

            var M   = " four".ToMaybeX();
            var lhs = M.SelectMany(i => i.ToMaybeX());
            var rhs = M;
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact][MsTest.TestMethod]
        public void MonadLaw3MaybeX() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            //Func<string,MaybeX<string>> addOne = x => x + 1;
            var M   = " four".ToMaybeX();
            var lhs = M.SelectMany(addOne).SelectMany(addEight);
            var rhs = M.SelectMany(x => addOne(x).SelectMany(addEight));
            Assert.True(lhs == rhs, description);
        }

        #if false
        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-MaybeX.html
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest1() {
            const int denominator = 2;
            var dt = DateTime.Now;
            Assert.Equal("Hello World! " + dt.ToShortDateString(),
                ( from a in "Hello World!".ToMaybeX()
                    from c in dt.ToMaybeX()
                    let sds = c.ToShortDateString()
                    select a + " " + sds
                ).ToNothingString() );
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        /// <remarks>
        /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-MaybeX.html
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest2() {
            const int denominator = 0;

            Assert.Equal("Nothing",
                ( from a in "Hello World!".ToMaybe()
                    from b in 12.DoSomeDivision(denominator,2)
                    from c in DateTime.Now.ToMaybe()
                    let sds = c.ToShortDateString()
                    select a + " " + b.ToString() + " " + sds
                ).ToNothingString() );
        }

        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        /// <remarks>
        /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-MaybeX.html
        /// </remarks>
        [Fact][MsTest.TestMethod]
        public void MikeHadlowTest3() {
            var dt = DateTime.Now;
            Assert.Equal("Hello World! 4 " + dt.ToShortDateString(),
                ( from a in "Hello World!".ToMaybe()
                    from b in 24.DoSomeDivision(2,3)
                    from c in dt.ToMaybe()
                    let sds = c.ToShortDateString()
                    select a + " " + b.ToString() + " " + sds
                ).ToNothingString() );
        }
        #endif
    }

    internal class ExternalState {
        private  int        _state;

        public ExternalState() { _state = -1;  }
        public  int GetState() { return ++_state; }
    }
}
