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

namespace PGSolutions.Monads {
    /// <summary>An immutable value-type Maybe{<typeparamref name="T"/>} monad.</summary>
    /// <typeparam name="T">The base type, which must be a class type, and will have the
    /// Equality definition track that of the base-type:
    /// Value-equality for string, reference equality for others except where overridden.
    /// </typeparam>
    /// <remarks>
    /// Being a value-type reduces memory pressure on <see cref="System.GC"/>.
    /// 
    /// Equality tracks the base type, with the further proviseo that two instances
    /// can only be equal when <see cref="HasValue"/> is true for both instances.
    /// </remarks>
    public struct X<T> : IEquatable<X<T>>,ISafeToString where T:class {
        ///<summary>Create a new MaybeX{T}.</summary>
        private X(T value) : this() { Value = value; }

        ///<summary>Returns whether this MaybeX{T} has a value.</summary>
        public bool HasValue => Value != null;
        internal  T Value { get; }

        ///<summary>Extract value of the <see cref="X{T}"/>, substituting <paramref name="default"/> as needed.</summary>
        ///<remarks>Substitutes for the ?? operator, which is unavailable for overload.</remarks>
        public static T operator | (X<T> maybe, T @default) => maybe.Value ?? @default;
        ///<summary>Extract value of the X{T}, substituting <paramref name="default"/> as needed.</summary>
        ///<remarks>
        ///     Use of the coalescing operator (ie the pipe operator '|') is preferred except when 
        ///     inter-operability with <see cref="Nullable{T}"/> is required.
        /// </remarks>
        [Obsolete("This method is a temporary work-around enabling consistent usage between Nullable<T> and Maybe<T> of the ?? operator.")]
        public T BitwiseOr(T @default) => this | @default;

        ///<summary>Amplifies a <typeparamref name="T"/> to a <see cref="X{T}"/>.</summary>
        public static implicit operator X<T>(T value) => new X<T>(value);

//        ///<summary>Returns the inner (type <typeparamref name="T"/>) value for those who like to live dangerously.</summary>
//        public static explicit operator T(X<T> value) => value.Value;

        #region Value Equality with IEquatable<T>.
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj as X<T>?)?.Equals(this) ?? false;

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        /// <remarks>Value-equality is tested if reference-equality fails but both sides are non-null.</remarks>
        public bool Equals(X<T> other)  =>
            Value != null ? other.Value != null && (Value == other.Value || Value.Equals(other.Value))
                          : other.Value == null;

        /// <summary>Tests value-equality, returning false if either value doesn't exist.</summary>
        public static bool operator == (X<T> lhs, X<T> rhs) => lhs.Equals(rhs);

        /// <summary>Tests value-inequality, returning false if either value doesn't exist..</summary>
        public static bool operator != (X<T> lhs, X<T> rhs) => ! lhs.Equals(rhs);

        ///<summary>Returns the hash code of the contained object, or zero if that is null.</summary>
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        /// <inheritdoc/>
        public override string ToString() => this.Select(v => v.ToString()) | "Nothing!";
        #endregion
    }

    /// <summary>Convenience extension methods for <see cref="X{T}"/></summary>
    public static class X {
        ///<summary>Amplifies a reference-type T to an <see cref="X{T}"/>.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static X<T>      New<T>(T value) where T:class => value;

        ///<summary>Amplifies a reference-type <typeparamref name="T"/> to an <see cref="X{T}"/>.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        ///<remarks>The monad <i>unit</i> function.</remarks>
        public static X<T>      AsX<T>(this T @this) where T:class => @this;

        ///<summary>"Boxes" a value-type <typeparamref name="T"/>, then amplifies it to an <see cref="X{T}"/>.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        ///<remarks>THe input value is first (presumably) "boxed" by the CLR.</remarks>
        public static X<object> ToX<T>(this T @this) where T:struct => @this;

        ///<summary>Returns the type of the underlying type {TValue}.</summary>
        ///<typeparam name="T">The type of the "contained" object, being amplified to an <see cref="X{T}"/></typeparam>
        ///<param name="this">todo: describe this parameter on GetUnderlyingType</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
        public static Type      GetUnderlyingType<T>(this X<T> @this) where T:class => typeof(T);
    }
}
