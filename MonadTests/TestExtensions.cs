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
using System.Diagnostics.Contracts;

namespace PGSolutions.Utilities.Monads.UnitTests {
    internal static class MaybeTestsExtensions {
        /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
        public static string ToNothingString<T>(this Maybe<T> @this) {
            Contract.Ensures(Contract.Result<string>() != null);
            return @this.SelectMany<string>(v => (Maybe<string>)v.ToString()) | "Nothing";
        }

        /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
        public static string ToNothingString<T>(this T? @this) where T:struct {
            Contract.Ensures(Contract.Result<string>() != null);
            return @this.HasValue ? @this.Value.ToString() : "Nothing";
        }

        /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
        public static string ToNothingString<T>(this MaybeX<T> @this
        ) where T:class {
            Contract.Ensures(Contract.Result<string>() != null);
            return @this.SelectMany<string>(v => v.ToString()) | "Nothing";
        }
    }

    [Pure]
    internal static partial class EnumerableExtensions {
        public static IEnumerable<T> Enumerable<T>(this T value) {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
            yield return value;
        }
    }
}
