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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System;
using System.Globalization;

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>TODO</summary>
    [Pure]
    public static class StringExtensions {
        /// <summary>Shortcut for string.Format(CultureInfo.InvariantCulture, @this, arg).</summary>
        /// <param name="this">The Format string.</param>
        /// <param name="arg">The arguments to be formatted into the format string.</param>
        public static string    FormatMe(this string @this, params object[] arg) {
            @this.ContractedNotNull(nameof(@this));
            arg.ContractedNotNull(nameof(arg));
            Ensures(Result<string>() != null);

            return @this.FormatMe(CultureInfo.InvariantCulture, arg);
        }

        /// <summary>Shortcut for string.Format(cultureInfo, @this, arg).</summary>
        /// <param name="this">The Format string.</param>
        /// <param name="arg">The arguments to be formatted into the format string.</param>
        /// <param name="cultureInfo">TODO</param>
        public static string    FormatMe(this string @this,
            IFormatProvider cultureInfo,
            params object[] arg
        ) {
            @this.ContractedNotNull(nameof(@this));
            cultureInfo.ContractedNotNull(nameof(cultureInfo));
            arg.ContractedNotNull(nameof(arg));
            Ensures(Result<string>() != null);

            return string.Format(cultureInfo, @this, arg);
        }
    }

    /// <summary>Constant utility functions.</summary>
    [Pure]
    public static class Functions {
        /// <summary>The identity function - Returns its argument.</summary>
        public static TValue    Identity<TValue>(TValue value) {
            value.ContractedNotNull(nameof(value));
            Ensures(Result<TValue>() != null);
            return value;
        }

        /// <summary>Returns its first argument.</summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "second")]
        public static TFirst    First<TFirst,TSecond> (TFirst first, TSecond second) {
            first.ContractedNotNull(nameof(first));
            Ensures(Result<TFirst>() != null);

            return first;
        }

        /// <summary>Returns its second argument.</summary>
        public static TSecond   Second<TFirst,TSecond>(TFirst first, TSecond second) {
            second.ContractedNotNull(nameof(second));
            Ensures(Result<TSecond>() != null);

            return second;
        }
    }
}
