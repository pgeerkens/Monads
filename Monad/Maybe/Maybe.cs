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

    /// <summary>An immutable value-type Maybe&lt;T> monad.</summary>
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
        ///<summary>Create a new Maybe&lt;T>.</summary>
        public Maybe(T value) : this() {
            value.ContractedNotNull("value");
            Contract.Ensures((_value==null) == (value==null));
            Contract.Ensures(HasValue == (value!=null));
            Contract.Ensures((_value != null) || !ValueIsStruct);

            _value    = value;
            _hasValue = true;
        }

        /// <summary>The Invalid data value.</summary>
        public static Maybe<T> Nothing { get { return _nothing; }
        } static readonly Maybe<T> _nothing = new Maybe<T>();

            ///<summary>Returns whether this Maybe&lt;T> has a value.</summary>
            public bool HasValue { [Pure]get {return _hasValue;} } readonly bool _hasValue;

        ///<summary>The monadic Bind operation from type T to type 
        ///Maybe&lt;TResult>.</summary>
        [Pure]
        public Maybe<TResult>       Bind<TResult>(
            Func<T, Maybe<TResult>> selector
        ) {
            selector.ContractedNotNull("selector");

            Maybe<TResult>.Nothing.AssumeInvariant();
            return ! HasValue  ?  Maybe<TResult>.Nothing  :  selector(Value);
        }

        ///<summary>Extract value of the Maybe&lt;T>, substituting 
        ///<paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public T                    Extract(T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<T>() != null);

            return ! HasValue  ?  defaultValue  :  Value;
        }
        ///<summary>Extract value of the Maybe&lt;T>, substituting 
        ///<paramref name="defaultValue"/> as needed.</summary>
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
            Contract.Invariant( HasValue == (Value != null) );
            Contract.Invariant( this != null );
            Contract.Invariant((_value != null)  ||  !ValueIsStruct);
        }

        /// <inheritdoc/>
        [Pure]public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return Bind<string>(v => v.ToString()) | "";
        }
        #region Non-Core
        /// <summary>Optimized implementation of Map.</summary>
        ///<remarks>Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        public Maybe<TResult>             Map<TResult>(
            Func<T,TResult> projector
        ) {
            projector.ContractedNotNull("projector");
            return ! HasValue  ?  Maybe<TResult>.Nothing  :  projector(_value);
        }

        /// <summary>LINQ-compatible implementation of Flatten.</summary>
        public Maybe<TResult>             Flatten< TSelection, TResult>(
            Func<T, Maybe<TSelection>> selector,
            Func<T,TSelection,TResult> projector
        ) {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");

            var @this = this;
            return ! HasValue ? Maybe<TResult>.Nothing
                                : selector(Value).Select(e => projector(@this._value,e));
        }

        ///<summary>Wraps a T as a Maybe&lt;T>.</summary>
        [Pure]
        public static implicit operator Maybe<T>(T value) { 
            Maybe<T>.Nothing.AssumeInvariant();
            var result = value == null ? Maybe<T>.Nothing : new Maybe<T>(value);

            return result;
        }

        public Maybe<TValue>       ToMaybe<TValue>(Maybe<TValue> maybe
        ) {
            maybe.AssumeInvariant();
            Maybe<TValue>.Nothing.AssumeInvariant();
            return ! maybe.HasValue ? Maybe<TValue>.Nothing : maybe.Value;
        }

        /// <summary>Re-wraps a <typeparamref name="TValue"/> from a Maybe to a MaybeX.</summary>
        public MaybeX<TValue>      ToMaybeX<TValue>(Maybe<TValue> maybe
        ) where TValue : class {
            //@this.ContractedNotNull("this");
            Contract.Ensures(Contract.Result<MaybeX<TValue>>() != null); 

            maybe.AssumeInvariant();
            MaybeX<TValue>.Nothing.AssumeInvariant();
            return ! maybe.HasValue ? MaybeX<TValue>.Nothing : maybe.Value;
        }

        ///<summary>If this Maybe&lt;T> has a value, returns it.</summary>
        private  T Value    {
            [Pure]get { Contract.Requires(HasValue);
                        Contract.Ensures((Contract.Result<T>()==null) == (_value==null));  
                        return _value;}
        } readonly   T  _value;

        [Pure]
        public static implicit operator Maybe<T>(Maybe<Maybe<T>> value) {
            return value.HasValue && value.Value.HasValue ? value.Value : Maybe<T>.Nothing;
        }

        ///<summary>Returns the type of the underlying type &lt.TValue>.</summary>
        public Type GetUnderlyingType {
            get { Contract.Ensures(Contract.Result<System.Type>() != null);
                    return typeof(T);}
        }
        #endregion
        #region Value Equality with IEquatable<T> and "excluded middle" present w/ either side has no value.
        #region static support for IEquatable<Maybe<T>>
        #region implicit static constructor
        static readonly bool            _valueIsStruct  = typeof(ValueType).IsAssignableFrom(typeof(T));
        static readonly Func<T,T,bool>  _equals         = ValueIsStruct ? (Func<T,T,bool>)ValEquals 
                                                                        : (Func<T,T,bool>)RefEquals;
        #endregion

        [Pure]private static bool ValEquals(T lhs, T rhs) { return lhs.Equals(rhs); }
        [Pure]private static bool RefEquals(T lhs, T rhs) {
            return typeof(string).IsAssignableFrom(typeof(T)) ? lhs.Equals(rhs)
                                                            : object.ReferenceEquals(lhs, rhs);
        }

        [Pure]static bool  ValueIsStruct { get {return _valueIsStruct;} }
        #endregion

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) { 
            var other = obj as Maybe<T>?;
            return other.HasValue  &&  other.Equals(obj);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(Maybe<T> other) {
            return this.HasValue  &&  other.HasValue  &&  _equals(this._value,other._value);
        }
        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool NotEquals(Maybe<T> other) {
            return this.HasValue  &&  other.HasValue  &&  ! _equals(this._value,other._value);
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() { return HasValue ? Value.GetHashCode() : 0; }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator == (Maybe<T> lhs, Maybe<T> rhs) { return lhs.Equals(rhs); }

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator != (Maybe<T> lhs, Maybe<T> rhs) { return lhs.NotEquals(rhs); }
        #endregion
    }

    public static class Maybe {
        public static Maybe<Unit> Unit { get { return PGSolutions.Utilities.Monads.Unit._.ToMaybe();} }

        ///<summary>Amplifies a reference-type TValue to a MaybeX&lt;TValue>.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static Maybe<TValue>       ToMaybe<TValue>(this TValue @this
        ) {
            @this.ContractedNotNull("this");
            //Contract.Ensures(Contract.Result<Maybe<TValue>>() != null); 

            return @this; 
        }

        ///<summary>Extract value of the Maybe&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static  TStruct            Extract<TStruct>
            (this Maybe<TStruct> @this) where TStruct:struct { return @this | default(TStruct); }

        ///<summary>Extract value of the Maybe&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static  Func<TStruct>      Extract<TStruct>
            (this Maybe<Func<TStruct>> @this) where TStruct:struct {
            Contract.Ensures(Contract.Result<Func<TStruct>>() != null);

            return @this | (()=>default(TStruct));
        }
    }
}
