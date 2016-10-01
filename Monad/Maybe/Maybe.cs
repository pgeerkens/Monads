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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGSolutions.Monads {
    using static Contract;
#if true
    /// <summary>An immutable value-type Maybe{T} monad.</summary>
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
    public struct Maybe<TValue> : IEquatable<Maybe<TValue>> {
        /// <summary>The Invalid data value.</summary>
        [Pure]
        public static Maybe<TValue> Nothing { get; } = default(Maybe<TValue>);

        ///<summary>Create a new Maybe{T}.</summary>
        private Maybe(TValue value) : this() {
            Ensures(!HasValue ||  Value != null);
            //Ensures(HasValue.Implies(Value != null));    // works in MayceX.cs, not here.

            Value    = value;
            HasValue = Value != null;
        }

        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ <i>let</i> clause and queries with a single FROM clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public Maybe<TResult>   Select<TResult>(
            Func<TValue, TResult> projector
        ) {
            projector.ContractedNotNull(nameof(projector));

            return ! HasValue ? default(Maybe<TResult>) : (Maybe<TResult>)projector(Value);
        }

        ///<summary>The monadic Bind operation of type T to type Maybe{TResult}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public Maybe<TResult>   SelectMany<TResult>(
            Func<TValue, Maybe<TResult>> selector
        ) {
            selector.ContractedNotNull(nameof(selector));

            var value = Value;
            return ! HasValue ? default(Maybe<TResult>) : selector(value);
        }

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        [Pure]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public Maybe<TResult>   SelectMany<T, TResult>(
            Func<TValue, Maybe<T>> selector,
            Func<TValue, T, TResult> projector
        ) {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));

            var value = Value;
            return ! HasValue ? default(Maybe<TResult>)
                              : selector(value).Select(e => projector(value, e));
        }

        ///<summary>Returns whether this Maybe{T} has a value.</summary>
        public bool HasValue { get; }

        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public TValue BitwiseOr(TValue defaultValue) {
            defaultValue.ContractedNotNull(nameof(defaultValue));
            Ensures(Result<TValue>() != null);

            return ! HasValue ? defaultValue : Value;
        }
        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static TValue operator | (Maybe<TValue> value, TValue defaultValue) {
            defaultValue.ContractedNotNull(nameof(defaultValue));
            Ensures(Result<TValue>() != null);

            return value.BitwiseOr(defaultValue);
        }

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Invariant(Value != null || !HasValue);
            //Invariant(HasValue.Implies(_value != null));   // works in MayceX.cs, not here.
        }

        ///<summary>Wraps a T as a Maybe{T}.</summary>
        [Pure]
        public static explicit operator Maybe<TValue>(TValue value) => new Maybe<TValue>(value);

        private TValue Value { get; }

        ///<summary>Returns the type of the underlying type {TValue}.</summary>
        [Pure]
        public Type GetUnderlyingType {
            get { Ensures(Result<Type>() != null);  return typeof(TValue); }
        }

#region Value Equality with IEquatable<T> and "excluded middle" present w/ either side has no value.
#region implicit static constructor
        static readonly bool _valueIsStruct = typeof(ValueType).IsAssignableFrom(typeof(TValue))
                                                     || typeof(string).IsAssignableFrom(typeof(TValue));
        static readonly Func<TValue, TValue, bool> _valEquals = (TValue lhs, TValue rhs) => lhs.Equals(rhs);
        static readonly Func<TValue, TValue, bool> _refEquals = (TValue lhs, TValue rhs) => ReferenceEquals(lhs, rhs);
        static readonly Func<TValue, TValue, bool> _equals = _valueIsStruct ? _valEquals : _refEquals;
#endregion

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) => (obj as Maybe<TValue>?)?.Equals(this) ?? false;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(Maybe<TValue> other) =>
            HasValue ? other.HasValue && _equals(Value, other.Value)
                     : !other.HasValue;

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator ==(Maybe<TValue> lhs, Maybe<TValue> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator !=(Maybe<TValue> lhs, Maybe<TValue> rhs) => !lhs.Equals(rhs);

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public bool? AreNonNullEqual(Maybe<TValue> rhs) =>
            HasValue && rhs.HasValue ? Value.Equals(rhs.Value)
                                     : null as bool?;

        ///<summary>Tests value-inequality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public bool? AreNonNullUnequal(Maybe<TValue> rhs) =>
            HasValue && rhs.HasValue ? !Value.Equals(rhs.Value)
                                     : null as bool?;
#endregion

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Ensures(Result<string>() != null);
            return SelectMany<string>(v => v.ToString().ToMaybe()) | "";
        }
    }

    /// <summary>TODO</summary>
    [Pure]
    public static class Maybe {
        ///<summary>Amplifies a reference-type T to a Maybe{T}.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static Maybe<TValue> ToMaybe<TValue>(this TValue @this) =>
            @this == null ? default(Maybe<TValue>) : (Maybe<TValue>)@this;

        ///<summary>Extract value of the Maybe{T}, substituting a defaultValue(<typeparam name="TStruct"/>) as needed.</summary>
        public static Func<TStruct> Extract<TStruct>(this Maybe<Func<TStruct>> @this) where TStruct : struct {
            Ensures(Result<Func<TStruct>>() != null);
            return @this | (() => default(TStruct));
        }

        /// <summary>TODO</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Maybe<T> Cast<T>(this Maybe<object> @this) => //where T : class =>
            from o in @this select (T)o;
    }
#endif
}
