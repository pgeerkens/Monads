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

namespace PGSolutions.Monads {
    //using static Contract;

    /// <summary>TODO</summary>
    [Pure]
    public static class Nullable {
        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ <i>let</i> clause and queries with a single FROM clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static TResult?   Select<T, TResult>(this T? @this,
            Func<T, TResult> projector
        ) where T : struct where TResult : struct {
            projector.ContractedNotNull(nameof(projector));

            return !@this.HasValue ? default(TResult?) : projector(@this.Value);
        }

        ///<summary>The monadic Bind operation of type T to type {TResult?}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static TResult?   SelectMany<T, TResult>(this T? @this,
            Func<T, TResult?> selector
        ) where T : struct where TResult : struct {
            selector.ContractedNotNull(nameof(selector));

            return !@this.HasValue ? default(TResult?) : selector(@this.Value);
        }

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple <i>from</i> clauses or with more complex structure.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static TResult?   SelectMany<T, TIntermediate, TResult>(this T? @this,
            Func<T, TIntermediate?> selector,
            Func<T, TIntermediate, TResult> projector
        ) where T : struct where TIntermediate : struct where TResult : struct {
            selector.ContractedNotNull(nameof(selector));
            projector.ContractedNotNull(nameof(projector));

            return !@this.HasValue ? default(TResult?)
                                   : selector(@this.Value).Select(e => projector(@this.Value, e));
        }

        ///<summary>The monadic Bind operation of type {T?} to type MaybeX{TResult?}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static Maybe<TResult>   SelectMany<T, TResult>(this T? @this,
            Func<T, Maybe<TResult>> selector
        ) where T : struct where TResult : struct {
            selector.ContractedNotNull(nameof(selector));

            return !@this.HasValue ? default(Maybe<TResult>) : selector(@this.Value);
        }

        ///<summary>The monadic Bind operation of type {T?} to type MaybeX{TResult?}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static MaybeX<TResult>   SelectMany<T, TResult>(this T? @this,
            Func<T, MaybeX<TResult>> selector
        ) where T : struct where TResult : class {
            selector.ContractedNotNull(nameof(selector));

            return !@this.HasValue ? default(MaybeX<TResult>) : selector(@this.Value);
        }

        ///<summary>Amplifies a value-type T to a {T}?.</summary>
        public static T? AsMaybe<T>(this T @this) where T : struct => @this;

        /// <summary>TODO</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Extract<T>(this T? @this, T defaultValue) where T : struct =>
            @this.HasValue ? @this.Value : defaultValue;
    }
}
