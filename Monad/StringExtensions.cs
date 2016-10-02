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
using System.Diagnostics.Contracts;
using System.Globalization;

namespace PGSolutions.Monads {
    using static CultureInfo;

    /// <summary>Convenience extension methods for formatting strings.</summary>
    [Pure]
    public static class StringExtensions {
        /// <summary>Shortcut for string.Format(CultureInfo.InvariantCulture, @this, arg).</summary>
        /// <param name="this">The Format string.</param>
        /// <param name="arg">The arguments to be formatted into the format string.</param>
        public static string    FormatMe(this string @this, params object[] arg) =>
            @this?.FormatMe(InvariantCulture, arg);

        /// <summary>Shortcut for string.Format(cultureInfo, @this, arg).</summary>
        /// <param name="this">The Format string.</param>
        /// <param name="arg">The arguments to be formatted into the format string.</param>
        /// <param name="cultureInfo">TODO</param>
        public static string    FormatMe(this string @this,
            IFormatProvider cultureInfo,
            params object[] arg
        ) => @this==null || arg==null ? null : string.Format(cultureInfo, @this, arg);
    }
}