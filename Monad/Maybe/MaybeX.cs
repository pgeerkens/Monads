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
    public struct MaybeX<T> : IEquatable<MaybeX<T>> where T:class {
        private const string _ccCheckFailuare = 
            "ccCheck failure - struct's never null, and 'MaybeX<TResult>.Nothing.AssumeInvariant()' inadequate!";

        /// <summary>The Invalid Data value.</summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [Pure]
        public static MaybeX<T> Nothing {
            get { return _nothing; }
        } static readonly MaybeX<T> _nothing = new MaybeX<T>();

        ///<summary>Create a new MaybeX&lt;T>.</summary>
        public MaybeX(T value) : this() {
            Contract.Ensures(HasValue == (_value != null));
            Contract.Ensures( (_value == null)  == (value==null) );

            _value    = value;
            Contract.Assert(HasValue == (_value != null), "ccCheck failure - necessary for ObjectInvariant but trivial to prove.");
        }

        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ <i>let</i> clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        public MaybeX<TResult>  Select<TResult>(
            Func<T, TResult> projector
        ) where TResult : class {
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            return (_value == null) ? MaybeX<TResult>.Nothing : projector(_value);
        }

        ///<summary>The monadic Bind operation of type T to type MaybeX&lt;TResult>.</summary>
        /// <remarks>
        /// Used for LINQ queries with a single <i>from</i> clause.
        /// </remarks>
        [Pure]
        public  MaybeX<TResult> SelectMany<TResult>(
            Func<T, MaybeX<TResult>> selector
        ) where TResult:class {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            var result = (_value == null) ?  MaybeX<TResult>.Nothing  :  selector(_value);
        
            Contract.Assume(result != null, _ccCheckFailuare);
            return result;
        }

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        public MaybeX<TResult> SelectMany<TIntermediate, TResult>(
            Func<T, MaybeX<TIntermediate>> selector,
            Func<T,TIntermediate,TResult> projector
        ) where TIntermediate:class where TResult:class {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            var @this = this;
            return (_value == null) ? MaybeX<TResult>.Nothing
                                    : selector(_value).Select(e => projector(@this._value, e));
        }

        ///<summary>Extract value of the MaybeX&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public T Extract(T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<T>() != null);

            return _value ?? defaultValue;
        }
        ///<summary>Extract value of the MaybeX&lt;T>, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public static T operator | (MaybeX<T> value, T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Contract.Ensures(Contract.Result<T>() != null);

            return value.Extract(defaultValue);
        }

        ///<summary>Returns whether this MaybeX&lt;T> has a value.</summary>
        public bool    HasValue { [Pure]get { return _value != null;} }

        /////<summary>If this MaybeX&lt;T> has a value, returns it.</summary>
        readonly T _value;

        ///<summary>Wraps a T as a MaybeX&lt;T>.</summary>
        [Pure]
        public static implicit operator MaybeX<T>(T value) {
            Contract.Ensures(Contract.Result<MaybeX<T>>() != null);

            var result = new MaybeX<T>(value);

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

            return typeof(T);
        }

        #region Value Equality with IEquatable<T>.
        static readonly bool _valueIsString = typeof(string).IsAssignableFrom(typeof(T));

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) { 
            var other = obj as MaybeX<T>?;
            return other.HasValue  &&  Equals(other.Value);
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public bool Equals(MaybeX<T> other)  =>
               ( (_value == null) && (other._value == null))
            || ( (_value != null) && (other._value != null)
              && (_value == other._value || (_valueIsString && _value.Equals(other._value)))
               );

        ///<summary>Retrieves the hash code of the object returned by the <see cref="_value"/> property.</summary>
        [Pure]
        public override int GetHashCode() => (_value == null) ? 0 : _value.GetHashCode();

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator == (MaybeX<T> lhs, MaybeX<T> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator != (MaybeX<T> lhs, MaybeX<T> rhs) => ! lhs.Equals(rhs);

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public Maybe<bool> AreNonNullEqual(MaybeX<T> rhs) =>
            from t in ( from lv in this
                        from rv in rhs
                        select new { lv, rv }).ToMaybe()
            select t.lv.Equals(t.rv);

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        [Pure]
        public Maybe<bool> AreNonNullUnequal(MaybeX<T> rhs) =>
            from t in ( from lv in this
                        from rv in rhs
                        select new { lv, rv }).ToMaybe()
            select ! t.lv.Equals(t.rv);
        #endregion

        ///<summary>Amplifies a reference-type T to a MaybeX&lt;T>.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public Maybe<T> ToMaybe()  =>
            _value == null ? Maybe<T>.Nothing : new Maybe<T>(_value);

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return SelectMany<string>(v => v.ToString()) | "";
        }
    }
    [Pure]
    public static class MaybeX {
        ///<summary>Amplifies a reference-type T to a MaybeX&lt;T>.</summary>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static MaybeX<TValue> ToMaybeX<TValue>(this TValue @this)
        where TValue:class => new MaybeX<TValue>(@this); 
    }
}
