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

namespace PGSolutions.Monads {
    /// <summary>Class representing, conceptually, the "type" of <i>void</i>.</summary>
    public struct Unit : IEquatable<Unit>, IComparable<Unit> {
        /// <summary>The single instance of <see cref="Unit"/></summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [CLSCompliant(false)]
        public static Unit _ { get { return _this; } } static Unit _this = new Unit();

        #region Comparison with IComparable<T>
        /// <inheritdoc/>
        [Pure]public int CompareTo(Unit other) => 0;
        /// <summary>Returns true exactly when lhs &lt; rhs.</summary>
        [Pure]public static bool operator <(Unit lhs, Unit rhs) => lhs.CompareTo(rhs) < 0;
        /// <summary>Returns true exactly when lhs &gt; rhs.</summary>
        [Pure]public static bool operator >(Unit lhs, Unit rhs) => lhs.CompareTo(rhs) > 0;
        #endregion
        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]public override bool Equals(object obj) => obj is Unit;
        /// <summary>Returns true, as all Unit instances are equal.</summary>
        [Pure]public bool Equals(Unit other) => true;
        /// <summary>Returns true, as all Unit instances are equal.</summary>
        [Pure]public static bool operator ==(Unit lhs, Unit rhs) => lhs.Equals(rhs);
        /// <summary>Returns false, as all Unit instances are equal.</summary>
        [Pure]public static bool operator !=(Unit lhs, Unit rhs) => !lhs.Equals(rhs);

        /// <inheritdoc/>
        [Pure]public override int GetHashCode() => 0;
        #endregion
    }
}
