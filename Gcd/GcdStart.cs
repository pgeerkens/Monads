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
#define GCDStateAsClassX    // performance penalty of 30% to 200% for class compared to struct

using System;
using System.Diagnostics.Contracts;

namespace PGSolutions.Monads.Demos {
    using static Contract;
    using static String;

#if GCDStateAsClass
    public class GcdStart : IEquatable<GcdStart> {
#else
  /// <summary>TODO</summary>
    public struct GcdStart : IEquatable<GcdStart> {
#endif
   
        /// <summary>TODO</summary>
        public GcdStart(int a, int b) { A = a; B = b; }

        /// <summary>TODO</summary>
        public int A { get; }
        /// <summary>TODO</summary>
        public int B { get; }

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        public override string ToString() {
            Ensures(Result<string>() != null);
            return Format("({0,14:N0}, {1,14:N0})", A, B) ?? "";
        }

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
        #if GCDStateAsClass
              var other = obj as GcdStart;
              return other != null  &&  other.Equals(obj);
        #else
              var other = obj as GcdStart?;
              return other.HasValue && other.Equals(obj);
        #endif
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(GcdStart other) { return A == other.A && B == other.B; }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() { unchecked { return A.GetHashCode() ^ B.GetHashCode(); } }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator ==(GcdStart lhs, GcdStart rhs) { return lhs.Equals(rhs); }

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator !=(GcdStart lhs, GcdStart rhs) { return !lhs.Equals(rhs); }
        #endregion
    }
}
