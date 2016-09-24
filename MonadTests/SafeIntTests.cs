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
using System.Diagnostics.Contracts;
using System.Globalization;
using Xunit;

namespace PGSolutions.Monads.UnitTests {
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
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

    public static class StringExtensions {
        static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;
        static readonly CultureInfo _currentCulture   = CultureInfo.CurrentUICulture;

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj")]
        public static string ICFormat(this string format, params object[] objArray) {
            format.ContractedNotNull(nameof(format));
            objArray.ContractedNotNull(nameof(objArray));
            Contract.Ensures(Contract.Result<string>() != null);
            return string.Format(_invariantCulture, format, objArray);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj")]
        public static string CCFormat(this string format, params object[] objArray) {
            format.ContractedNotNull(nameof(format));
            objArray.ContractedNotNull(nameof(objArray));
            Contract.Ensures(Contract.Result<string>() != null);
            return string.Format(_currentCulture, format, objArray);
        }
    }
}
