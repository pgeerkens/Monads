// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
 
    using System;

namespace PGSolutions.Monads {
    using System.Collections.Generic;

    // Warning, don't put System.Runtime.Serialization.On*Serializ*Attribute
    // on this class without first fixing ObjectClone::InvokeVtsCallbacks
    // Also, because we have special type system support that says a a boxed Nullable2<T>
    // can be used where a boxed<T> is use, Nullable2<T> can not implement any intefaces
    // at all (since T may not).   Do NOT add any interfaces to Nullable2!
    // 
    /// <summary>TODO</summary>
    [Serializable]
    [System.Runtime.Versioning.NonVersionable] // This only applies to field layout
    public struct Nullable2<T> //where T : struct
    {
        private static bool isValueType = typeof(ValueType).IsAssignableFrom(typeof(T));
        private static Func<Nullable2<T>,bool> staticHasValue = @this => isValueType ? @this.hasValue
                                                                                     : @this.value!=null;
        private bool hasValue; 
        internal T value;

        /// <summary>TODO</summary>
        [System.Runtime.Versioning.NonVersionable]
        public Nullable2(T value) : this() {
            this.value = value;
            if (isValueType) this.hasValue = true;
        }

        /// <summary>TODO</summary>
        public bool HasValue {
            [System.Runtime.Versioning.NonVersionable]
            get {
                return staticHasValue(this);
            }
        }

        /// <summary>TODO</summary>
        [Obsolete("Use is strongly discouraged as violating the basic premise of avoiding unnecessary exceptions.")]
        public T Value => HasValue ? Value : throw new InvalidOperationException(ExceptionResource.InvalidOperation_NoValue);

        ///<summary>Extract value of the <see cref="X{T}"/>, substituting <paramref name="default"/> as needed.</summary>
        ///<remarks>Substitutes for the ?? operator, which is unavailable for overload.</remarks>
        public static T operator | (Nullable2<T> maybe, T @default) =>
            maybe.GetValueOrDefault(@default);
        ///<summary>Extract value of the X{T}, substituting <paramref name="default"/> as needed.</summary>
        ///<remarks>
        ///     Use of the coalescing operator (ie the pipe operator '|') is preferred except when 
        ///     inter-operability with <see cref="Nullable{T}"/> is required.
        /// </remarks>
        [Obsolete("This method is a temporary work-around enabling consistent usage between Nullable<T> and Maybe<T> of the ?? operator.")]
        public T BitwiseOr(T @default) => this | @default;

        /// <summary>TODO</summary>
        [System.Runtime.Versioning.NonVersionable]
        public T GetValueOrDefault() {
            return value;
        }

        /// <summary>TODO</summary>
        [System.Runtime.Versioning.NonVersionable]
        public T GetValueOrDefault(T defaultValue) {
            return HasValue ? value : defaultValue;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
        //    var rhs = obj as Nullable2<T>?;
            if (obj is Nullable2<T>) return Equals((Nullable2<T>)obj,this);
            else return false;
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        /// <remarks>Value-equality is tested if reference-equality fails but both sides are non-null.</remarks>
        public bool Equals(Nullable2<T> lhs, Nullable2<T> rhs) =>
            !lhs.HasValue ? !rhs.HasValue
                          : rhs.HasValue && (ReferenceEquals(lhs.value, rhs.value)
                                           || object.Equals((T)lhs.value, (T)rhs.value)
                                            );
        /// <summary>Tests value-equality.</summary>
        public static bool operator == (Nullable2<T> lhs, Nullable2<T> rhs) => lhs.Equals(rhs);
        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (Nullable2<T> lhs, Nullable2<T> rhs) => ! lhs.Equals(rhs);

        /// <summary>TODO</summary>
        public override int GetHashCode() {
            return HasValue ? value.GetHashCode() : 0;
        }

        /// <summary>TODO</summary>
        public override string ToString() {
            return HasValue ? value.ToString() : "";
        }

        /// <summary>TODO</summary>
        [System.Runtime.Versioning.NonVersionable]
        public static implicit operator Nullable2<T>(T value) {
            return new Nullable2<T>(value);
        }

        //#pragma warning disable CS0618
        ///// <summary>TODO</summary>
        //[System.Runtime.Versioning.NonVersionable]
        //public static explicit operator T(Nullable2<T> value) => value.Value;
        //#pragma warning restore CS0618
 
        // The following already obsoleted methods were removed:
        //   public int CompareTo(object other)
        //   public int CompareTo(Nullable2<T> other)
        //   public bool Equals(Nullable2<T> other)
        //   public static Nullable2<T> FromObject(object value)
        //   public object ToObject()
        //   public string ToString(string format)
        //   public string ToString(IFormatProvider provider)
        //   public string ToString(string format, IFormatProvider provider)
        
        // The following newly obsoleted methods were removed:
        //   string IFormattable.ToString(string format, IFormatProvider provider)
        //   int IComparable.CompareTo(object other)
        //   int IComparable<Nullable2<T>>.CompareTo(Nullable2<T> other)
        //   bool IEquatable<Nullable2<T>>.Equals(Nullable2<T> other)
    }

    /// <summary>TODO</summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public static class Nullable2 {

        /// <summary>TODO</summary>
        [System.Runtime.InteropServices.ComVisible(true)]
        public static int Compare<T>(Nullable2<T> n1, Nullable2<T> n2) where T : struct
        {
            if (n1.HasValue) {
                if (n2.HasValue) return Comparer<T>.Default.Compare(n1.value, n2.value);
                return 1;
            }
            if (n2.HasValue) return -1;
                return 0;
            }

        /// <summary>TODO</summary>
        [System.Runtime.InteropServices.ComVisible(true)]
        public static bool Equals<T>(Nullable2<T> n1, Nullable2<T> n2) where T : struct
        {
            if (n1.HasValue) {
                if (n2.HasValue) return EqualityComparer<T>.Default.Equals(n1.value, n2.value);
                return false;
                }
            if (n2.HasValue) return false;
                    return true;
                }

        // If the type provided is not a Nullable2 Type, return null.
        // Otherwise, returns the underlying type of the Nullable2 type
        /// <summary>TODO</summary>
        public static Type GetUnderlyingType(Type nullableType) {
            if((object)nullableType == null) {
                throw new ArgumentNullException("nullableType");
            }
            Type result = null;
            if( nullableType.IsGenericType && !nullableType.IsGenericTypeDefinition) { 
                // instantiated generic type only                
                Type genericType = nullableType.GetGenericTypeDefinition();
                if( Object.ReferenceEquals(genericType, typeof(Nullable2<>))) {
                    result = nullableType.GetGenericArguments()[0];
                }
            }
            return result;
        }

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public static Nullable2<bool> AreNonNullEqual<TValue>(this TValue lhs, TValue rhs)
            => lhs != null && rhs != null ? lhs.Equals(rhs) : new Nullable2<bool>();

        ///<summary>Tests value-equality, returning <b>Nothing</b> if either value doesn't exist.</summary>
        public static Nullable2<bool> AreNonNullUnequal<TValue>(this TValue lhs, TValue rhs)
            => lhs != null && rhs != null ? ! lhs.Equals(rhs) : new Nullable2<bool>();
 
        // The following already obsoleted methods were removed:
        //   public static bool HasValue<T>(Nullable2<T> value)
        //   public static T GetValueOrDefault<T>(Nullable2<T> value)
        //   public static T GetValueOrDefault<T>(Nullable2<T> value, T valueWhenNull)
 
        // The following newly obsoleted methods were removed:
        //   public static Nullable2<T> FromObject<T>(object value)
        //   public static object ToObject<T>(Nullable2<T> value)
        //   public static object Wrap(object value, Type type)
        //   public static object Unwrap(object value)
    }        
}
