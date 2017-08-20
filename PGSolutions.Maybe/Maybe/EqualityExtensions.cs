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

namespace PGSolutions.Monads {
    /// <summary>TODO</summary>
    public static class EqualityExtensions {
        /// <summary>A string representing the object's value, or "Nothing" if it has no value.</summary>
        public static string ToNothingString<T>(this T @this) {
            return @this != null ? @this.ToString() : "Nothing";
        }

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        public static bool?     AreNonNullEqual<T>(this X<T> lhs, X<T> rhs) where T:class =>
            from l in lhs from r in rhs from result in l.Equals(r) as bool? select result;

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        public static bool?     AreNonNullUnequal<T>(this X<T> lhs, X<T> rhs) where T:class =>
            from result in lhs.AreNonNullEqual(rhs) select ! result;

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="Nullable{T}"/></typeparam>
        public static bool?     AreNonNullEqual<T>(this T? lhs, T? rhs) where T:struct =>
            lhs.SelectMany(l => rhs, (l,r) => l.Equals(r));

        ///<summary>Tests value-equality, returning null if either value doesn't exist.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="Nullable{T}"/></typeparam>
        public static bool?     AreNonNullUnequal<T>(this T? lhs, T? rhs) where T:struct =>
            lhs.SelectMany(l => rhs, (l,r) => ! l.Equals(r));

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public static bool? AreNonNullEqual<TValue>(this TValue lhs, TValue rhs)
            => lhs != null && rhs != null ? lhs.Equals(rhs) : null as bool?;

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public static bool? AreNonNullUnequal<TValue>(this TValue lhs, TValue rhs)
            => lhs != null && rhs != null ? ! lhs.Equals(rhs) : null as bool?;
    }
}
