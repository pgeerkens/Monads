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

  /// <summary>An immutable value-type MaybeX&lt;T> monad.</summary>
  /// <typeparam name="TValue">The base type, which can be either a class or struct type,
  /// and will have the Equality definition track the default for the base-type:
  /// Value-equality for structs and string, reference equality for other classes.
  /// </typeparam>
  /// <remarks
  /// >Being a value-type reduces memory pressure on <see cref="System.GC"/>.
  /// 
  /// Equality tracks the base type (struct or class), with the further proviseo
  /// that two instances can only be equal when <see cref="HasValue"/> is true
  /// for both instances.
  /// </remarks>
  public struct MaybeX<TValue> : IEquatable<MaybeX<TValue>> where TValue:class {
    /// <summary>The Invalid Data value.</summary>
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static readonly MaybeX<TValue>      Nothing  = new MaybeX<TValue>(null); 

    ///<summary>Create a new MaybeX&lt;T>.</summary>
    public MaybeX(TValue value) : this() {
        Contract.Ensures(HasValue == (Value != null));
        Contract.Ensures( (Value==null)  == (value==null) );

        _value    = value;
        Contract.Assert(HasValue == (Value != null), "ccCheck failure - necessary for ObjectInvariant but trivial to prove.");
    }

    ///<summary>The monadic Bind operation of type T to type MaybeX&lt;TResult>.</summary>
    [Pure]
    public  MaybeX<TResult>               Bind<TResult>(
        Func<TValue, MaybeX<TResult>> selector
    ) where TResult:class {
        selector.ContractedNotNull("selector");
        Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

        MaybeX<TResult>.Nothing.AssumeInvariant();
        var result = ! HasValue  ?  MaybeX<TResult>.Nothing  :  selector(Value);
        
        Contract.Assume(result != null, "ccCheck failure - struct's never null, and AssumeInvariant inadequate!");
        return result;
    }

    ///<summary>Extract value of the MaybeX&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
    [Pure]
    public  TValue                        Extract(TValue defaultValue) {
        defaultValue.ContractedNotNull("defaultValue");
        Contract.Ensures(Contract.Result<TValue>() != null);

        return ! HasValue  ?  defaultValue  :  Value;
    }

    ///<summary>Optimized LINQ-compatible implementation of Bind/Map as Select.</summary>
    ///<remarks>Always available from Bind():
    ///         return @this.Bind(v => projector(v).ToMaybe());
    ///</remarks>
    public MaybeX<TResult>                Map<TResult>(
        Func<TValue,TResult> projector
    ) where TResult:class {
        //@this.ContractedNotNull("this");
        projector.ContractedNotNull("projector");
        Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

        var @this = this;
        @this.AssumeInvariant();
        MaybeX<TResult>.Nothing.AssumeInvariant();
        return ! HasValue ? MaybeX<TResult>.Nothing : projector(@this.Value);
    }

    ///<summary>LINQ-compatible implementation of Bind/FlatMap as SelectMany.</summary>
    ///<remarks>
    ///Set as internal to avoid confusion with the identically named LINQ method with
    ///different signature and database-semantics instead of Category-Theory-semantics.
    ///
    /// This method is accessible through its the LINQ alias SelectMany.
    ///</remarks>
    internal MaybeX<TResult>              Flatten<T, TResult>(
        Func<TValue, MaybeX<T>> selector,
        Func<TValue,T,TResult> projector
    ) where T:class where TResult:class {
        //@this.ContractedNotNull("this");
        selector.ContractedNotNull("selector");
        projector.ContractedNotNull("projector");
        Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

        MaybeX<TResult>.Nothing.AssumeInvariant();
        var @this = this;
        return ! HasValue ? MaybeX<TResult>.Nothing
                          : selector(Value).Map(e => projector(@this.Value,e));
    }

    ///<summary>Returns whether this MaybeX&lt;T> has a value.</summary>
    public    bool    HasValue {
        [Pure]get {Contract.Ensures(Contract.Result<bool>() == (Value != null)); return Value != null;}
    }

    ///<summary>If this MaybeX&lt;T> has a value, returns it.</summary>
    private  TValue  Value    {
      [Pure]get {Contract.Ensures(Contract.Result<TValue>() == _value);  return _value;}
    }
    readonly TValue _value;

    ///<summary>Wraps a T as a MaybeX&lt;T>.</summary>
    [Pure]
    public static implicit operator MaybeX<TValue>(TValue value) {
        Contract.Ensures(Contract.Result<MaybeX<TValue>>() != null);
        
        MaybeX<TValue>.Nothing.AssumeInvariant();
        var result = value == null ? MaybeX<TValue>.Nothing : new MaybeX<TValue>(value);

        Contract.Assume(result != null, "ccCheck failure - struct's never null, and AssumeInvariant inadequate!");
        return result;
    }

    ///<summary>The invariants enforced by this struct type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]private void ObjectInvariant() {
        Contract.Invariant( HasValue == (Value != null) );
        Contract.Invariant( Nothing != null );
        Contract.Invariant( this != null );
    }

    ///<summary>Returns the type of the underlying type &lt.TValue>.</summary>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public Type GetUnderlyingType() {
      Contract.Ensures(Contract.Result<Type>() != null);

      return typeof(TValue);
    }

    #region static support for IEquatable<MaybeX<T>>
    static readonly bool            _valueIsString  = typeof(string).IsAssignableFrom(typeof(TValue));
    [Pure]
    private static bool RefEquals(TValue lhs, TValue rhs) {
        return _valueIsString  ?  lhs.Equals(rhs)  :  object.ReferenceEquals(lhs, rhs);
    }
    #endregion

    #region Value Equality with IEquatable<T>.
    /// <inheritdoc/>
    [Pure]public override bool Equals(object obj) { 
      var other = obj as MaybeX<TValue>?;
      return other.HasValue  &&  other.Equals(obj);
    }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]public bool Equals(MaybeX<TValue> other) {
        //return this.HasValue  &&  other.HasValue  &&  _equals(this.Value,other.Value);
        return this.Value.Equals(other.Value);
    }

    ///<summary>Retrieves the hash code of the object returned by the <see cref="Value"/> property.</summary>
    [Pure]public override int GetHashCode() { return Value.GetHashCode(); }

    /// <inheritdoc/>
    [Pure]public override string ToString() {
      Contract.Ensures(Contract.Result<string>() != null);
      return Bind<string>(v => v.ToString()).Extract("");
    }

    /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
    [Pure]public static bool operator == (MaybeX<TValue> lhs, MaybeX<TValue> rhs) {
        return ( ! lhs.HasValue && ! rhs.HasValue )
            || ( lhs.HasValue == rhs.HasValue  
                  && ( lhs.Value==rhs.Value || ( _valueIsString && lhs.Value.Equals(rhs.Value)) )
               );
    }

    /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
    [Pure]public static bool operator != (MaybeX<TValue> lhs, MaybeX<TValue> rhs) {return ! (lhs == rhs);}
    #endregion
  }

  [Pure]
  public static class MaybeX {
    ///<summary>Amplifies a reference-type TValue to a MaybeX&lt;TValue>.</summary>
    ///<remarks>The monad <i>unit</i> function.</remarks>
    public static MaybeX<TValue>          ToMaybeX<TValue>(this TValue @this
    ) where TValue:class {
        @this.ContractedNotNull("this");
        Contract.Ensures(Contract.Result<MaybeX<TValue>>() != null); 

        return @this; 
    }

    ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
    public static Maybe<bool>             AreEqual<TValue>(this MaybeX<TValue> lhs, MaybeX<TValue> rhs
    ) where TValue : class {
        //lhs.ContractedNotNull("lhs");
        //rhs.ContractedNotNull("rhs");
        return !lhs.HasValue || !rhs.HasValue ? Maybe<bool>.Nothing : lhs.Equals(rhs);
    }

    ///<summary>Tests value-inequality, returning <b>Nothing</b> if either value doesn't exist.</summary>
    public static Maybe<bool>             AreUnequal<TValue>(this MaybeX<TValue> lhs, MaybeX<TValue> rhs
    ) where TValue : class {
        //lhs.ContractedNotNull("lhs");
        //rhs.ContractedNotNull("rhs");
        return !lhs.HasValue || !rhs.HasValue ? Maybe<bool>.Nothing : ! lhs.Equals(rhs);
    }
  }
}
