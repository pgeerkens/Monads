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

        /// <inheritdoc/>
        [Pure]public int CompareTo(Unit other) => 0;
        /// <summary>Returns true exactly when lhs &lt; rhs.</summary>
        [Pure]public static bool operator <(Unit lhs, Unit rhs) => lhs.CompareTo(rhs) < 0;
        /// <summary>Returns true exactly when lhs &gt; rhs.</summary>
        [Pure]public static bool operator >(Unit lhs, Unit rhs) => lhs.CompareTo(rhs) > 0;

        #region Value Equality with IEquatable<T>.
        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]public bool Equals(Unit other) => true;
        /// <inheritdoc/>
        [Pure]public override bool Equals(object obj) => obj is Unit;
        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]public static bool operator ==(Unit lhs, Unit rhs) => lhs.Equals(rhs);
        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]public static bool operator !=(Unit lhs, Unit rhs) => !lhs.Equals(rhs);

        /// <inheritdoc/>
        [Pure]public override int GetHashCode() => 0;
        #endregion
    }

    /// <summary>Class representing, conceptually, the "type" of <i>void</i>.</summary>
    [Pure]
    public static class UnitExtensionsLinq {
        readonly static Func<Unit> _nothing = null;
        /// <summary>The LINQ-enabling Select method.</summary>
        public static Func<Unit>     Select(this Unit @this,
            Func<Unit, Unit> projector
        ) =>
            projector==null ? _nothing : () => projector(@this);

        /// <summary>The monadic bind method.</summary>
        public static Func<Unit>     SelectMany(this Unit @this,
            Func<Unit, Func<Unit>> selector
        ) =>
            selector==null ? _nothing : () => selector(@this)();

        /// <summary>The LINQ-enabling SelectMany method.</summary>
        public static Func<Unit>     SelectMany<TSelection>(this Unit @this,
            Func<Unit, Func<TSelection>> selector,
            Func<Unit,TSelection,Unit>   projector
        ) =>
            selector==null || projector==null ? _nothing 
                                              : () => projector(@this, selector(@this)() );
    }
}
