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
using System.Linq;

using Xunit;

namespace PGSolutions.Monads.MonadTests {
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors",
        Justification ="Unit tests require a public default constructor.")]
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeTests {
        public MaybeTests() {
            _maybeGeorge = "George".ToMaybe();
            _data        = ( from e in new List<string>() { "Percy", null, "George","Ron", "Ginny" }
                             select e.ToMaybe()
                           ).ToList().AsReadOnly();
            _addOne      = x => (Maybe<int>)(x + 1);
            _addEight    = x => (Maybe<int>)(x + 8);
            _datetime    = DateTime.Now; //Convert.ToDateTime("04/01/2016 1:10:35 PM");
        }

        readonly Maybe<string>         _maybeGeorge;
        readonly IList<Maybe<string>>  _data;
        readonly Func<int, Maybe<int>> _addOne;
        readonly Func<int, Maybe<int>> _addEight;
        readonly DateTime              _datetime;

        [Theory]
        [InlineData("",       "Percy//George/Ron/Ginny")]
        [InlineData("Nothing","Percy/Nothing/George/Ron/Ginny")]
        public void BasicTest(string defaultValue, string expected) {
            var received = string.Join("/", from e in _data
                                            select e | defaultValue
                                            );
            Contract.Assert(received != null);
            Assert.Equal(expected, received);
        }

        /// <summary>Verify that == and != are opposites, and are implemented as Equals().</summary>
        [Theory]
        [InlineData(true,  "George")]
        [InlineData(false, "Percy/Nothing/Ron/Ginny")]
        public void IncludedMiddleTest(bool comparison, string expected) {
            expected.ContractedNotNull(nameof(expected));
            var received = string.Join("/", from e in _data
                                            where e.Equals(_maybeGeorge) == comparison
                                            select e.ToNothingString()
                                            );
            Assert.True(expected == received);
            Assert.False(ReferenceEquals(expected, received));

            received     = string.Join("/", from e in _data
                                            where (e == _maybeGeorge) == comparison
                                            select e.ToNothingString()
                                            );
            Assert.Equal(expected, received);

            received     = string.Join("/", from e in _data
                                            where (e != _maybeGeorge) != comparison
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
                                            where e.AreNonNullEqual(_maybeGeorge) == comparison
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

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public void MonadLaw1Maybe() {
            const string description = "Monad law 1: m.Monad().Bind(f) == f(m)";

            var lhs = 1.ToMaybe().SelectMany(_addOne);
            var rhs = _addOne(1);
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2Maybe() {
            const string description = "Monad law 2: M.Bind(Monad) == M";

            var M = 4.ToMaybe();
            var lhs = M.SelectMany(i => i.ToMaybe());
            var rhs = M;
            Assert.True(lhs == rhs, description);
        }

        /// <summary>Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))</summary>
        [Fact]
        public void MonadLaw3Maybe() {
            const string description = "Monad law 3: M.Bind(f1).Bind(f2) == M.Bind(x => f1(x).Bind(f2))";

            var M = 4.ToMaybe();
            var lhs = M.SelectMany(_addOne).SelectMany(_addEight);
            var rhs = M.SelectMany(x => _addOne(x).SelectMany(_addEight));
            Assert.True(lhs == rhs, description);
        }
    }
}
