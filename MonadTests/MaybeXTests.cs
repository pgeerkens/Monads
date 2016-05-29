﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

using Xunit;
//using MsTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGSolutions.Utilities.Monads.UnitTests {
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeXTests {
        public MaybeXTests() {
            _data        = ( from e in new List<string>() { "Percy", null, "George", "Ron", "Ginny" }
                             select e.AsMaybeX()
                           ).ToList().AsReadOnly();
            _addOne      = x => x + "constant";
            _addEight    = x => x + "/" + x;
            _datetime    = DateTime.Now;
        }

        readonly IList<MaybeX<string>>        _data;
        readonly Func<string, MaybeX<string>> _addOne;
        readonly Func<string, MaybeX<string>> _addEight;
        readonly DateTime                     _datetime;

        [Theory]
        [InlineData("",        "Percy//George/Ron/Ginny")]
        [InlineData("Nothing", "Percy/Nothing/George/Ron/Ginny")]
        public void BasicTest(string defaultValue, string expected) {
            var received = string.Join("/", from e in _data
                                            select e | defaultValue
                                            );
            Contract.Assert(received != null);
            Assert.Equal(expected, received);
        }

        /// <summary>Verify that == and != are opposites, and are implemented as Equals().</summary>
        [Theory]
        [InlineData(true, "George")]
        [InlineData(false, "Percy/Nothing/Ron/Ginny")]
        public void IncludedMiddleTest(bool comparison, string expected) {
            expected.ContractedNotNull(nameof(expected));
            var received = string.Join("/", from e in _data
                                            where e.Equals("George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.True(expected == received);
            Assert.False(ReferenceEquals(expected, received));

            received     = string.Join("/", from e in _data
                                            where (e == "George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);

            received     = string.Join("/", from e in _data
                                            where (e != "George") != comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Ron/Ginny")]
        [InlineData(null,  "Nothing")]
        public void ExcludedMiddleTest(bool? comparison, string expected) {
            var received = string.Join("/", from e in _data
                                            where e.AreNonNullEqual("George") == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);
        }

        [Fact]
        public static void LazyEvaluationTest() {
            var state = new ExternalState();
            var x = (from a in (Maybe<Func<int>>)state.GetState select a).Extract();
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Assert.Equal(0, y);
            Assert.Equal(6, state.GetState());
            Assert.Equal(7, x());
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Theory]
        [InlineData("Fred Weasley", " Weasley")]
        [InlineData("Nothing",      null)]
        public static void WesDyerTest(string expected, string second) {
            var received = ( from x in "Fred".AsMaybeX()
                             from y in second.AsMaybeX()
                             select x + y).ToNothingString();
            Assert.Equal(expected, received);
        }

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public void MonadLaw1() {
            const string description = "Monad law 1: m.Monad().Bind(f) == f(m)";

            var lhs = "1".AsMaybeX().SelectMany(_addOne);
            var rhs = _addOne("1");
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2() {
            const string description = "Monad law 2: M.Bind(Monad) == M";

            var M = " four".AsMaybeX();
            var lhs = M.SelectMany(i => i.AsMaybeX());
            var rhs = M;
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public void MonadLaw3() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            //Func<string,MaybeX<string>> addOne = x => x + 1;
            var M = " four".AsMaybeX();
            var lhs = M.SelectMany(_addOne).SelectMany(_addEight);
            var rhs = M.SelectMany(x => _addOne(x).SelectMany(_addEight));
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-MaybeX.html
        /// </remarks>
        [Theory]
        [InlineData(4, true, "Hello World! 6 ")]
        [InlineData(0, false, "Nothing")]
        [InlineData(6, true,  "Hello World! 4 ")]
        public void MikeHadlowTest(int denominator, bool append, string expected) {
            if (append) expected = expected + _datetime.ToShortDateString();

            var received = ( from a in "Hello World!".AsMaybeX()
                             from b in ((SafeInt)24 / denominator).Value.SelectMany(v=>v.ToMaybeX())
                             from c in ((object)_datetime).AsMaybeX()
                             let sds = ((DateTime)c).ToShortDateString()
                             select a + " " +  (int)b + " " + sds
                           ).ToNothingString();

            Assert.Equal(expected, received);
        }
    }
}
