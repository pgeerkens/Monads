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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using Xunit;

namespace PGSolutions.Monads.MonadTests {
    using static CultureInfo;
    using static String;
    using static Functions;

    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MixedMaybeTests {
        #region static fields
        readonly static string           _maybeGeorge = "George";
        readonly static string[]         _strings     = { "Percy", null, "George","Ron", "Ginny" };
        readonly static IList<string>    _data        = _strings.ToList().AsReadOnly();
        readonly static Func<int, int?>  _addOne      = x => (x + 1);
        readonly static Func<int, int?>  _addEight    = x => (x + 8);
        readonly static Func<int,string> _concatEight = i => Format(InvariantCulture,"{0}eight",i);

        readonly Func<string,Func<string,bool>,IEnumerable<string>> IsGeorge = (s,test) => 
                from e in _data where test(e) select e ?? s;
        #endregion

        [Theory]
        [InlineData("",       "Percy//George/Ron/Ginny")]
        [InlineData("Nothing","Percy/Nothing/George/Ron/Ginny")]
        public void BasicTest(string defaultValue, string expected) {
            Func<string,bool> predicate;
            string received;

            predicate = s => true;
            received  = Join("/", IsGeorge(defaultValue, predicate) );
            Contract.Assert(received != null);
            Assert.True(expected?.Equals(received), Format("Value: Expected: '{0}'; Received: '{1}'",expected,received));
        }

        /// <summary>Verify that == and != are opposites, and are implemented as Equals().</summary>
        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Nothing/Ron/Ginny")]
        public void IncludedMiddleTest(bool comparison, string expected) {
            expected.ContractedNotNull(nameof(expected));
            Func<string,bool> predicate;
            string received;

            predicate = s => (s == _maybeGeorge) == comparison;
            received  = Join("/", IsGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);

                predicate = s => (s != _maybeGeorge) != comparison;
                received  = Join("/", IsGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);
        }

        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Ron/Ginny")]
        [InlineData(null,  "Nothing")]
        public void ExcludedMiddleTest(bool? comparison, string expected) {
            Func<string,bool> predicate;
            string received;

            predicate = s => s.AreNonNullEqual(_maybeGeorge) == comparison;
            received  = Join("/", IsGeorge("Nothing", predicate) );
            Assert.Equal(expected, received);
        }

        [Fact]
        public static void LazyEvaluationTest() {
            var state = new ExternalState();
            var x = (from a in (X<Func<int>>)state.GetState select a) | (()=>0);
            var y = x();

            for (int i = 0; i++ < 5;) state.GetState();

            Assert.Equal(0, y);
            Assert.Equal(6, state.GetState());
            Assert.Equal(7, x());
        }

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public void MonadLaw1Maybe() {
            const string description = "Monad law 1: m.Monad().Bind(f) == f(m)";

            var M   = 1.ToNullable();
            var lhs = M.SelectMany(_addOne, Second);
            var rhs = _addOne(1);
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2Maybe() {
            const string description = "Monad law 2: M.Bind(Monad) == M";

            var M   = 4.ToNullable();
            var lhs = M.SelectMany(i => i.ToNullable(), Second);
            var rhs = M;
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public void MonadLaw3Maybe() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            var M   = 4.ToNullable();
            var lhs = M.SelectMany(_addOne).SelectMany(_addEight);
            var rhs = M.SelectMany(m => _addOne(m).SelectMany(_addEight));
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public void MonadLaw3MaybeMixed() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            var M = 4 as int?;
            var addOneX = X.New<Func<int,int>>(i => i+1);
            var lhs = from m in M from a in addOneX select _concatEight(a(m));
            var rhs = from m in M select from a in addOneX select _concatEight(a(m));
            Assert.True(lhs == rhs, description);

            lhs = M.SelectMany(a => addOneX, (m,a) => _concatEight(a(m)));
            rhs = M.Select(m => addOneX.Select(a => _concatEight(a(m))) );
            Assert.True(lhs == rhs, description);
        }
    }
}
