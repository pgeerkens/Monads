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

namespace PGSolutions.Monads {
    /// <summary>TODO</summary>
    public class Box<T> where T:struct {
        /// <summary>TODO</summary>
        /// <param name="t"></param>
        public Box(T t) {Value = t;}

        /// <summary>TODO</summary>
        public T Value { get; }

        /// <summary>TODO</summary>
        public static implicit operator Box<T>(T value) => new Box<T>(value);

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        /// <remarks>Value-equality is tested if reference-equality fails but both sides are non-null.</remarks>
        public bool Equals(Box<T> other) => other?.Value.Equals(Value) ?? false;

        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj as Box<T>)?.Equals(this) ?? false;

        /// <summary>Tests value-equality, returning false if either value doesn't exist.</summary>
        public static bool operator ==(Box<T> lhs, Box<T> rhs) => lhs?.Equals(rhs) ?? false;

        /// <summary>Tests value-inequality, returning false if either value doesn't exist..</summary>
        public static bool operator !=(Box<T> lhs, Box<T> rhs) => ! (lhs == rhs);

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();
    }

    /// <summary>TODO</summary>
    public static class Box {
        /// <summary>TODO</summary>
        public static Box<T> ToBox<T>(this T t) where T : struct => new Box<T>(t);
    }
}
