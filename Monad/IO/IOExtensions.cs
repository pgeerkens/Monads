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
using System.Diagnostics.Contracts;

namespace PGSolutions.Monads {
    using static Contract;

    /// <summary>TODO</summary>
    /// <remarks>
    /// This pretty much comes straight from Dixin's Blog:
    ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
    /// except for all the Contract verification and some code reformatting.
    /// </remarks>
    [Pure]
    public static class IOExtensions {
        /// <summary>TODO</summary>
        public static IO<Unit> Action
          (Action action) {
            action.ContractedNotNull(nameof(action));
            return action.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T, IO<Unit>> Action<T>
          (this Action<T> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T, IO<Unit>>>() != null);
            return action.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, IO<Unit>> Action<T1, T2>
          (this Action<T1, T2> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, IO<Unit>>>() != null);
            return action.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, IO<Unit>> Action<T1, T2, T3>
          (this Action<T1, T2, T3> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, T3, IO<Unit>>>() != null);
            return action.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, T4, IO<Unit>> Action<T1, T2, T3, T4>
          (this Action<T1, T2, T3, T4> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, T3, T4, IO<Unit>>>() != null);
            return action.AsIO();
        }

        // ...

        /// <summary>TODO</summary>
        public static IO<T> Func<T>
          (this Func<T> function) {
            function.ContractedNotNull(nameof(function));
            return function.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T, IO<TResult>> Func<T, TResult>
          (this Func<T, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T, IO<TResult>>>() != null);
            return function.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, IO<TResult>> Func<T1, T2, TResult>
          (this Func<T1, T2, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, IO<TResult>>>() != null);
            return function.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, IO<TResult>> Func<T1, T2, T3, TResult>
          (this Func<T1, T2, T3, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, T3, IO<TResult>>>() != null);
            return function.AsIO();
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, T4, IO<TResult>> Func<T1, T2, T3, T4, TResult>
          (this Func<T1, T2, T3, T4, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, T3, T4, IO<TResult>>>() != null);
            return function.AsIO();
        }

        // ...

        /// <summary>TODO</summary>
        public static IO<Unit> AsIO
          (this Action action) {
            action.ContractedNotNull(nameof(action));
        //    Ensures(Result<IO<Unit>>() != null);

            return new IO<Unit>( () => { action(); return Unit._; });
        }

        /// <summary>TODO</summary>
        public static Func<T, IO<Unit>> AsIO<T>
          (this Action<T> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T, IO<Unit>>>() != null);

            return arg => 
                new IO<Unit>(() => { action(arg); return Unit._; } );
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, IO<Unit>> AsIO<T1, T2>
          (this Action<T1, T2> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, IO<Unit>>>() != null);

            return (arg1, arg2) => 
                new IO<Unit>(() => { action(arg1, arg2); return Unit._; } );
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, IO<Unit>> AsIO<T1, T2, T3>
          (this Action<T1, T2, T3> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, T3, IO<Unit>>>() != null);

            return (arg1, arg2, arg3) =>
                new IO<Unit>(() => { action(arg1, arg2, arg3); return Unit._; } );
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, T4, IO<Unit>> AsIO<T1, T2, T3, T4>
          (this Action<T1, T2, T3, T4> action) {
            action.ContractedNotNull(nameof(action));
            Ensures(Result<Func<T1, T2, T3, T4, IO<Unit>>>() != null);

            return (arg1, arg2, arg3, arg4) =>
                new IO<Unit>(() => { action(arg1, arg2, arg3, arg4); return Unit._; } );
        }

        // ...

        /// <summary>TODO</summary>
        public static IO<TResult> AsIO<TResult>
          (this Func<TResult> function) {
            function.ContractedNotNull(nameof(function));
        //    Ensures(Result<IO<TResult>>() != null);

            return new IO<TResult>(() => function());
        }

        /// <summary>TODO</summary>
        public static Func<T, IO<TResult>> AsIO<T, TResult>
          (this Func<T, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T, IO<TResult>>>() != null);

            return arg =>
                new IO<TResult>(() => function(arg));
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, IO<TResult>> AsIO<T1, T2, TResult>
          (this Func<T1, T2, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, IO<TResult>>>() != null);

            return (arg1, arg2) =>
                new IO<TResult>(() => function(arg1, arg2));
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, IO<TResult>> AsIO<T1, T2, T3, TResult>
          (this Func<T1, T2, T3, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, T3, IO<TResult>>>() != null);

            return (arg1, arg2, arg3) =>
                new IO<TResult>(() => function(arg1, arg2, arg3));
        }

        /// <summary>TODO</summary>
        public static Func<T1, T2, T3, T4, IO<TResult>> AsIO<T1, T2, T3, T4, TResult>
          (this Func<T1, T2, T3, T4, TResult> function) {
            function.ContractedNotNull(nameof(function));
            Ensures(Result<Func<T1, T2, T3, T4, IO<TResult>>>() != null);

            return (arg1, arg2, arg3, arg4) =>
                new IO<TResult>(() => function(arg1, arg2, arg3, arg4));
        }
    }
}
