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
using System.Globalization;
using Xunit;

namespace PGSolutions.Monads {
    [CLSCompliant(false)]
    public static class SafeIntTests {
        #region Addition Tests
        [Theory]
        [InlineData( 1,int.MaxValue, +1, null)]
        [InlineData( 2,int.MaxValue,  0, int.MaxValue)]
        [InlineData( 3,int.MaxValue, -1, int.MaxValue-1)]

        [InlineData( 4,int.MaxValue-1, +1, int.MaxValue)]
        [InlineData( 5,int.MaxValue-1,  0, int.MaxValue-1)]
        [InlineData( 6,int.MaxValue-1, -1, int.MaxValue-2)]

        [InlineData( 7,int.MinValue+1, +1, int.MinValue+2)]
        [InlineData( 8,int.MinValue+1,  0, int.MinValue+1)]
        [InlineData( 9,int.MinValue+1, -1, int.MinValue)]

        [InlineData(10,int.MinValue, +1, int.MinValue+1)]
        [InlineData(11,int.MinValue,  0, int.MinValue)]
        [InlineData(12,int.MinValue, -1, null)]

        [InlineData(13,int.MinValue/2-1, int.MinValue/2, null)]
        [InlineData(14,int.MinValue/2+0, int.MinValue/2, int.MinValue)]
        [InlineData(15,int.MinValue/2+1, int.MinValue/2, int.MinValue+1)]

        [InlineData(16,int.MaxValue/2+0, int.MaxValue/2, int.MaxValue-1)]
        [InlineData(17,int.MaxValue/2+1, int.MaxValue/2, int.MaxValue)]
        [InlineData(18,int.MaxValue/2+2, int.MaxValue/2, null)]
        #endregion
        public static void AdditionTests(int i,int? op1, int? op2, int? e) {
            const string format = "#{0}{1}: expected={2}; received={3}";
            var lhs = (SafeInt)op1; var rhs = (SafeInt)op2;
            var expected = (SafeInt)e;
            var received = lhs + rhs;
            Assert.True(expected.Equals(received), format.ICFormat(i,'A',expected,received));

            received = rhs + lhs;
            Assert.True(expected.Equals(received), format.ICFormat(i,'B', expected, received));
        }

        #region Subtraction Tests
        [Theory]
        [InlineData( 1,int.MaxValue, -2, null,           null)]
        [InlineData( 2,int.MaxValue, -1, null,           int.MinValue)]
        [InlineData( 3,int.MaxValue,  0, int.MaxValue,   int.MinValue+1)]
        [InlineData( 4,int.MaxValue, +1, int.MaxValue-1, int.MinValue+2)]

        [InlineData( 5,int.MaxValue-1, -3, null,           null)]
        [InlineData( 6,int.MaxValue-1, -2, null,           int.MinValue)]
        [InlineData( 7,int.MaxValue-1, -1, int.MaxValue,   int.MinValue+1)]
        [InlineData( 8,int.MaxValue-1,  0, int.MaxValue-1, int.MinValue+2)]

        [InlineData( 9,int.MinValue+1, -1, int.MinValue+2, int.MaxValue-1)]
        [InlineData(10,int.MinValue+1,  0, int.MinValue+1, int.MaxValue)]
        [InlineData(11,int.MinValue+1, +1, int.MinValue,   null)]
        [InlineData(12,int.MinValue+1, +2, null,           null)]

        [InlineData(13,int.MinValue, -2, int.MinValue+2, int.MaxValue-1)]
        [InlineData(14,int.MinValue, -1, int.MinValue+1, int.MaxValue)]
        [InlineData(15,int.MinValue,  0, int.MinValue,   null)]
        [InlineData(16,int.MinValue, +1, null,           null)]
        #endregion
        public static void SubtractionTests(int i, int? op1, int? op2, int? e1, int? e2) {
            const string format = "#{0}{1}: expected={2}; received={3}";
            var lhs = (SafeInt)op1; var rhs = (SafeInt)op2;
            var expected = (SafeInt)e1;
            var received = lhs - rhs;
            Assert.True(expected.Equals(received), format.ICFormat(i,'A', expected, received));

            expected = (SafeInt)e2;
            received = rhs - lhs;
            Assert.True(expected.Equals(received), format.ICFormat(i,'B', expected, received));
        }
    }

    public static partial class StringExtensions {
        static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;
        static readonly CultureInfo _currentCulture   = CultureInfo.CurrentUICulture;

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj")]
        public static string ICFormat(this string format, params object[] array) {
            format.ContractedNotNull(nameof(format));
            array.ContractedNotNull(nameof(array));
            return string.Format(_invariantCulture, format, array);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj")]
        public static string CCFormat(this string format, params object[] array) {
            format.ContractedNotNull(nameof(format));
            array.ContractedNotNull(nameof(array));
            return string.Format(_currentCulture, format, array);
        }
#if false
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
        /// <summary>Chaining with LINQ Comprehension syntax: one invalid</summary>
        /// <remarks>
        /// after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-maybe.html
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [Theory]
        [InlineData(4, true,  "Hello World! 6 ")]
        [InlineData(0, false, "Nothing")]
        [InlineData(6, true,  "Hello World! 4 ")]
        public void MikeHadlowTest(int denominator, bool append, string expected) {
            if (append) expected = expected + _datetime.ToShortDateString();

            var received = ( from a in "Hello World!".ToMaybe()
                             from b in ((SafeInt)24 / denominator).Value//.SelectMany<int,int>(v => v.ToMaybe())
                             from c in _datetime.ToMaybe()
                             let sds = c.ToShortDateString()
                             select a + " " + b + " " + sds
                           ).ToNothingString();

            Assert.Equal(expected, received);
        }

        /// <summary>Chaining with LINQ Comprehension syntax: all valid</summary>
        /// <remarks>
        /// after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        /// </remarks>
        [Theory]
        [InlineData("12",      7)]
        [InlineData("Nothing", null)]
        public static void WesDyerTest(string expected, int? second) {
            var received = ( from y in second select 5 + y).ToNothingString();
            Assert.Equal(expected, received);
        }
#endif
    }
}
