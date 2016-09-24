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
    using static Contract;

    /// <summary>Class representing, conceptually, the "type" of <i>void</i>.</summary>
    public struct Unit : IEquatable<Unit>, IComparable<Unit> {
        /// <summary>The single instance of <see cref="Unit"/></summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [CLSCompliant(false)]
        public static Unit _ { get {return _this;} } static Unit _this = new Unit();

        /// <inheritdoc/>
        public int CompareTo(Unit other) { Ensures(Result<int>()==0); return 0; }
        /// <summary>TODO</summary>
        public static bool operator <(Unit lhs, Unit rhs) {
            Ensures(Result<bool>() == false);

            return lhs.CompareTo(rhs) < 0;
        }
        /// <summary>TODO</summary>
        public static bool operator >(Unit lhs, Unit rhs) {
            Ensures(Result<bool>() == false);

            return lhs.CompareTo(rhs) > 0;
        }

        #region Value Equality with IEquatable<T>.
        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]public bool Equals(Unit other) => true;

        /// <inheritdoc/>
        [Pure]public override bool Equals(object obj) => obj is Unit;

        /// <inheritdoc/>
        [Pure]public override int  GetHashCode() { Ensures(Result<int>()==0); return 0; }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]public static bool operator == (Unit lhs, Unit rhs) =>lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]public static bool operator != (Unit lhs, Unit rhs) => ! lhs.Equals(rhs);
        #endregion

        /// <summary>TODO</summary>
        public Func<Unit>     Select(
            Func<Unit, Unit> projector
        ) { 
            projector.ContractedNotNull(nameof(projector));
            Ensures(Result<Func<Unit>>() != null);

            var @this = this;
            return () => projector(@this);
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Func<Unit>     SelectMany(
            Func<Unit, Func<Unit>> selector
        ) {
            selector.ContractedNotNull(nameof(selector));
            Ensures(Result<Func<Unit>>() != null);

            var @this = this;
            return () => selector(@this)();
        }

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Func<Unit>     SelectMany<TSelection>(
            Func<Unit, Func<TSelection>> selector,
            Func<Unit,TSelection,Unit> projector
        ) {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));
            Ensures(Result<Func<Unit>>() != null);

            var @this = this;
            return () => projector(@this, selector(@this)() );
        }
    }
}
