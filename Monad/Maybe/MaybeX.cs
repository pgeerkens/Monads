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
        private const string _ccCheckFailuare = 
            "ccCheck failure - struct's never null, and 'MaybeX<TResult>.Nothing.AssumeInvariant()' inadequate!";

        /// <summary>The Invalid Data value.</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [Pure]
        public static MaybeX<TValue> Nothing {
            get { return _nothing; }
        } static readonly MaybeX<TValue> _nothing = new MaybeX<TValue>();

        ///<summary>Create a new MaybeX&lt;T>.</summary>
        public MaybeX(TValue value) : this() {
            Contract.Ensures(HasValue == (_value != null));
            Contract.Ensures( (_value == null)  == (value==null) );

            _value    = value;
            Contract.Assert(HasValue == (_value != null), "ccCheck failure - necessary for ObjectInvariant but trivial to prove.");
        }

        ///<summary>The monadic Bind operation of type T to type MaybeX&lt;TResult>.</summary>
        [Pure]
        public  MaybeX<TResult>     Bind<TResult>(
            Func<TValue, MaybeX<TResult>> selector
        ) where TResult:class {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            var result = (_value == null) ?  MaybeX<TResult>.Nothing  :  selector(_value);
        
            Contract.Assume(result != null, _ccCheckFailuare);
            return result;
        }

        ///<summary>Optimized LINQ-compatible implementation of Bind/Map as Select.</summary>
        ///<remarks>Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///
        /// This method is accessible through its the LINQ alias Select.
        ///</remarks>
        public MaybeX<TResult>      Map<TResult>(
            Func<TValue,TResult> projector
        ) where TResult:class {
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            return (_value == null) ? MaybeX<TResult>.Nothing : projector(_value);
        }

        ///<summary>LINQ-compatible implementation of Bind/FlatMap as SelectMany.</summary>
        ///<remarks>
        ///Set as internal to avoid confusion with the identically named LINQ method with
        ///different signature and database-semantics instead of Category-Theory-semantics.
        ///
        /// This method is accessible through its the LINQ alias SelectMany.
        ///</remarks>
        internal MaybeX<TResult>    Flatten<T, TResult>(
            Func<TValue, MaybeX<T>> selector,
            Func<TValue,T,TResult> projector
        ) where T:class where TResult:class {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            var @this = this;
            return (_value == null) ? MaybeX<TResult>.Nothing
                                    : selector(_value).Map(e => projector(@this._value, e));
        }

        ///<summary>Extract value of the MaybeX&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public TValue Extract(TValue defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<TValue>() != null);

            return _value ?? defaultValue;
        }
        ///<summary>Extract value of the MaybeX&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static TValue operator | (MaybeX<TValue> value, TValue defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<TValue>() != null);

            return value.Extract(defaultValue);
        }

        ///<summary>Returns whether this MaybeX&lt;T> has a value.</summary>
        public bool    HasValue { [Pure]get { return _value != null;} }

        /////<summary>If this MaybeX&lt;T> has a value, returns it.</summary>
        readonly TValue _value;

        ///<summary>Wraps a T as a MaybeX&lt;T>.</summary>
        [Pure]
        public static implicit operator MaybeX<TValue>(TValue value) {
            Contract.Ensures(Contract.Result<MaybeX<TValue>>() != null);

            var result = new MaybeX<TValue>(value);

            Contract.Assume(result != null, _ccCheckFailuare);
            return result;
        }

        ///<summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]private void ObjectInvariant() {
            Contract.Invariant( HasValue == (_value != null) );
            Contract.Invariant( Nothing != null );
        }

        ///<summary>Returns the type of the underlying type &lt.TValue>.</summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [Pure]
        public Type GetUnderlyingType() {
            Contract.Ensures(Contract.Result<Type>() != null);

            return typeof(TValue);
        }

        #region static support for IEquatable<MaybeX<T>>
        static readonly bool            _valueIsString  = typeof(string).IsAssignableFrom(typeof(TValue));
        [Pure]
        private static bool RefEquals(TValue lhs, TValue rhs) =>
                _valueIsString  ?  lhs.Equals(rhs)  :  object.ReferenceEquals(lhs, rhs);
        #endregion

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) { 
            var other = obj as MaybeX<TValue>?;
            return other.HasValue  &&  Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(MaybeX<TValue> rhs)  =>
               ( (this._value == null) && (rhs._value == null))
            || ( (this._value != null) && (rhs._value != null)
              && (this._value == rhs._value || ( _valueIsString && this._value.Equals(rhs._value)) )
               );

        ///<summary>Retrieves the hash code of the object returned by the <see cref="_value"/> property.</summary>
        [Pure]
        public override int GetHashCode() => (_value == null) ? 0 : _value.GetHashCode();

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return Bind<string>(v => v.ToString()) | "";
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator == (MaybeX<TValue> lhs, MaybeX<TValue> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator != (MaybeX<TValue> lhs, MaybeX<TValue> rhs) => ! lhs.Equals(rhs);
        #endregion

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public Maybe<bool> AreNonNullEqual(MaybeX<TValue> rhs) =>
            this._value == null || rhs._value == null ? Maybe<bool>.Nothing
                                                      : this._value == rhs._value;

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public Maybe<bool> AreNonNullUnequal(MaybeX<TValue> rhs) =>
            this._value == null || rhs._value == null ? Maybe<bool>.Nothing
                                                      : this._value != rhs._value;
    }
    [Pure]
    public static class MaybeX {
        ///<summary>Amplifies a reference-type TValue to a MaybeX&lt;TValue>.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static MaybeX<TValue>    ToMaybeX<TValue>(this TValue @this
        ) where TValue:class {
            @this.ContractedNotNull("this");
            Contract.Ensures(Contract.Result<MaybeX<TValue>>() != null); 

            return @this; 
        }

    }
}
