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

namespace PGSolutions.Utilities.Monads {

    /// <summary>An immutable value-type Maybe{T} monad.</summary>
    /// <typeparam name="T">The base type, which can be either a class or struct type,
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
    public struct Maybe<T> : IEquatable<Maybe<T>> {
        /// <summary>The Invalid data value.</summary>
        public static Maybe<T> Nothing { get { return _nothing; }
        } static readonly Maybe<T> _nothing = new Maybe<T>();

        ///<summary>Create a new Maybe{T}.</summary>
        public Maybe(T value) : this() {
            value.ContractedNotNull("value");
            Contract.Ensures((_value==null) == (value==null));
            Contract.Ensures(HasValue == (value!=null));
            Contract.Ensures((_value != null) || !_valueIsStruct);

            _value    = value;
            _hasValue = true;
        }

        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ <i>let</i> clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        public Maybe<TResult> Select<TResult>(
            Func<T, TResult> projector
        ) {
            projector.ContractedNotNull("projector");

            return !HasValue ? Maybe<TResult>.Nothing : projector(_value);
        }

        ///<summary>The monadic Bind operation of type T to type MaybeX&lt;TResult>.</summary>
        /// <remarks>
        /// Used for LINQ queries with a single <i>from</i> clause.
        /// </remarks>
        [Pure]
        public Maybe<TResult> SelectMany<TResult>(
            Func<T, Maybe<TResult>> selector
        ) {
            selector.ContractedNotNull("selector");

            return ! HasValue  ?  Maybe<TResult>.Nothing  :  selector(_value);
        }

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        public Maybe<TResult> SelectMany<TIntermediate, TResult>(
            Func<T, Maybe<TIntermediate>> selector,
            Func<T, TIntermediate, TResult> projector
        ) {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");

            var @this = this;
            return !HasValue ? Maybe<TResult>.Nothing
                             : selector(_value).Select(e => projector(@this._value, e));
        }

        ///<summary>Returns whether this Maybe{T} has a value.</summary>
        public bool HasValue { [Pure]get {return _hasValue;} } readonly bool _hasValue;

        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public T Extract(T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<T>() != null);

            return ! HasValue  ?  defaultValue  : _value;
        }
        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static T operator | (Maybe<T> value, T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<T>() != null);

            return value.Extract(defaultValue);
        }

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Contract.Invariant( HasValue == (_value != null) );
            Contract.Invariant( this != null );
            Contract.Invariant((_value != null)  ||  ! _valueIsStruct);
        }

        ///<summary>Wraps a T as a Maybe{T}.</summary>
        [Pure]
        public static implicit operator Maybe<T>(T value) { 
            Maybe<T>.Nothing.AssumeInvariant();
            var result = value == null ? Maybe<T>.Nothing : new Maybe<T>(value);

            return result;
        }

        /// <summary>Re-wraps a <typeparamref name="TValue"/> from a Maybe to a MaybeX.</summary>
        public MaybeX<TValue>      ToMaybeX<TValue>(Maybe<TValue> maybe) where TValue : class =>
            ! maybe.HasValue ? MaybeX<TValue>.Nothing : maybe._value;

        private readonly T  _value;

        ///<summary>Returns the type of the underlying type &lt.TValue>.</summary>
        [Pure]
        public Type GetUnderlyingType {
            get { Contract.Ensures(Contract.Result<System.Type>() != null);
                  return typeof(T);}
        }

        #region Value Equality with IEquatable<T> and "excluded middle" present w/ either side has no value.
        #region implicit static constructor
        static readonly bool             _valueIsStruct = typeof(ValueType).IsAssignableFrom(typeof(T))
                                                       || typeof(string).IsAssignableFrom(typeof(T));
        static readonly Func<T, T, bool> _valEquals     = (T lhs, T rhs) => lhs.Equals(rhs);
        static readonly Func<T, T, bool> _refEquals     = (T lhs, T rhs) => ReferenceEquals(lhs, rhs);
        static readonly Func<T,T,bool>   _equals        = _valueIsStruct ? _valEquals  : _refEquals;
        #endregion

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) { 
            var other = obj as Maybe<T>?;
            return other.HasValue && Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(Maybe<T> other) =>
            HasValue ? other.HasValue && _equals(_value, other._value)
                     : ! other.HasValue;
            //   ( ! HasValue  &&  ! other.HasValue )
            //|| ( HasValue  &&  other.HasValue  &&  _equals(_value, other._value) );

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HasValue ? _value.GetHashCode() : 0;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator == (Maybe<T> lhs, Maybe<T> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator != (Maybe<T> lhs, Maybe<T> rhs) => ! lhs.Equals(rhs);

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public bool? AreNonNullEqual(Maybe<T> rhs) =>
            this.HasValue && rhs.HasValue ? this._value.Equals(rhs._value)
                                          : null as bool?;

        ///<summary>Tests value-inequality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public bool? AreNonNullUnequal(Maybe<T> rhs) =>
            this.HasValue && rhs.HasValue ? !this._value.Equals(rhs._value)
                                          : null as bool?;
        #endregion

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return SelectMany<string>(v => v.ToString()) | "";
        }
    }

    public static class Maybe {
        public static Maybe<Unit> Unit { get { return PGSolutions.Utilities.Monads.Unit._.ToMaybe();} }

        ///<summary>Amplifies a reference-type T to a Maybe{T}.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static Maybe<TValue> ToMaybe<TValue>(this TValue @this) =>
            @this==null ? Maybe<TValue>.Nothing : new Maybe<TValue>(@this);

        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static  Func<TStruct>  Extract<TStruct>(this Maybe<Func<TStruct>> @this) where TStruct:struct {
            Contract.Ensures(Contract.Result<Func<TStruct>>() != null);
            return @this | (()=>default(TStruct));
        }
    }
}
