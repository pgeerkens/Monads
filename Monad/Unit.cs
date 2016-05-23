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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
    using static Contract;

    /// <summary>Class representing, conceptually, the "type" of <i>void</i>.</summary>
    public struct Unit : IEquatable<Unit> {
    /// <summary>The single instance of <see cref="Unit"/></summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [CLSCompliant(false)]
    public static Unit _ { get {return _this;} } static Unit _this = new Unit();

    #region Value Equality with IEquatable<T>.
    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]public bool Equals(Unit other) { return true; }

    /// <inheritdoc/>
    [Pure]public override bool Equals(object obj) { 
      var other = obj as Unit?;
      return other != null  &&  other.Equals(obj);
    }

    /// <inheritdoc/>
    [Pure]public override int GetHashCode() { Ensures(Result<int>()==0); return 0; }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]public static bool operator == (Unit lhs, Unit rhs) { return lhs.Equals(rhs); }

    /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
    [Pure]public static bool operator != (Unit lhs, Unit rhs) { return ! lhs.Equals(rhs); }
    #endregion
  }

  public static class UnitExtensions {
    public static Func<Unit>           Select(this Func<Unit> action, 
        Func<Unit, Unit> projector
    ) { 
        action.ContractedNotNull("action");
        projector.ContractedNotNull("projector");
        Ensures(Result<Func<Unit>>() != null);

        return () => projector(action());
    }
    public static Func<Unit>           SelectMany(this Func<Unit> action, 
        Func<Unit, Func<Unit>> selector
    ) {
        action.ContractedNotNull("action");
        selector.ContractedNotNull("selector");
        Requires(selector(Unit._) != null);
        Ensures(Result<Func<Unit>>() != null);

        var unit = action();
        return () => selector(unit)();
    }
    public static Func<Unit>           SelectMany<TSelection>(Func<Unit> action,
        Func<Unit, Func<TSelection>> selector,
        Func<Unit,TSelection,Unit> projector
    ) {
        action.ContractedNotNull("action");
        selector.ContractedNotNull("selector");
        projector.ContractedNotNull("projector");
        Ensures(Result<Func<Unit>>() != null);

        var unit = action();
        return () => projector(unit, selector(unit)() );
    }
  }
}
