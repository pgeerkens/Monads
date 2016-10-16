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

using PGSolutions.Monads;
using Xunit;

namespace PGSolutions.Monads.MonadTests {
    using static Functions;

    /// <summary>Unit test null arguments to <see cref="MaybeLinq"/> extension methods.</summary>
    [ExcludeFromCodeCoverage] [CLSCompliant(false)]
    public class MaybeLinqNullTests {
        /// <summary>Unit-test constructor.</summary>
        public MaybeLinqNullTests() { ; }

        static readonly int? _nullable = 7;

        [Fact]
        public void NullableSelect() {
            var received = _nullable.Select((Func<int,int>)null);
            Assert.True(received == null);
        }
        [Fact]
        public static void NullableSelectMany1Arg() {
            var received = _nullable.SelectMany((Func<int,int?>)null);
            Assert.True(received == null);
        }
        [Fact]
        public static void NullableSelectMany2Arg() {
            var received = _nullable.SelectMany((Func<int,int?>)null, Second);
            Assert.True(received == null, "1st arg null");

            received     = _nullable.SelectMany(u=>2, (Func<int,int,int>)null);
            Assert.True(received == null, "2nd arg null");
        }
        //----------------------------------------------------------------------
        [Fact]
        public static void NullableMixedSelectMany1Arg() {
            var received = _nullable.SelectMany((Func<int,X<string>>)null);
            Assert.True(received == null);
        }
        [Fact]
        public static void NullableMixedSelectMany2Arg() {
            var received = _nullable.SelectMany((Func<int,X<string>>)null, Second);
            Assert.True(received == null, "1st arg null");

            received     = _nullable.SelectMany(u=>"2", (Func<int,string,string>)null);
            Assert.True(received == null, "2nd arg null");
        }
        //======================================================================
        static readonly X<string> _maybe  = "Maybe?";
        [Fact]
        public void MaybeXSelect() {
            var received = _maybe.Select<string,string>(null);
            Assert.True(received == null);
        }
        [Fact]
        public static void MaybeXSelectMany1Arg() {
            var received = _maybe.SelectMany((Func<string,X<string>>)null);
            Assert.True(received == null);
        }
        [Fact]
        public static void MaybeXSelectMany2Arg() {
            var received = _maybe.SelectMany((Func<string,X<string>>)null, Second);
            Assert.True(received == null, "1st arg null");

            received     = _maybe.SelectMany(u=>"2", (Func<string,string,string>)null);
            Assert.True(received == null, "2nd arg null");
        }
        //----------------------------------------------------------------------
        [Fact]
        public static void MaybeMixedSelectMany2Arg() {
            var received = _maybe.SelectMany((Func<string,int?>)null, Second);
            Assert.True(received == null, "1st arg null");

            received     = _maybe.SelectMany(u=>2, (Func<string,int,int>)null);
            Assert.True(received == null, "2nd arg null");
        }
    }
}
