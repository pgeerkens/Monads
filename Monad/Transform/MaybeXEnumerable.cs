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

namespace PGSolutions.Monads {
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    /// <summary>TODO</summary>
    public static class MaybeXEnumerable {
        /// <summary>LINQ-compatible implementation of the monadic map operator.</summary>
        ///<remarks>
        /// Used to implement the LINQ LET clause and queries with a single FROM clause.
        /// 
        /// Always available from Bind():
        ///         return @this.Bind(v => projector(v).ToMaybe());
        ///</remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static MaybeX<IEnumerable<TResult>> Select<T, TResult>(this
            MaybeX<IEnumerable<T>> @this,
            MaybeX<Func<IEnumerable<T>, IEnumerable<TResult>>> projector
        ) =>
            from e in @this
            from p in projector
            select p(e);

        ///<summary>The monadic Bind operation of type T to type MaybeX{IEnumerable{TResult}}.</summary>
        /// <remarks>
        /// Convenience method - not used by LINQ
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static MaybeX<IEnumerable<TResult>> SelectMany<T, TResult>(this
            MaybeX<IEnumerable<T>> @this,
            MaybeX<Func<IEnumerable<T>, MaybeX<IEnumerable<TResult>>>> selector
        ) =>
            from e in @this
            from s in selector
            from r in s(e)
            select r;

        /// <summary>LINQ-compatible implementation of the monadic join operator.</summary>
        /// <remarks>
        /// Used for LINQ queries with multiple FROM clauses or with more complex structure.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static MaybeX<IEnumerable<TResult>> SelectMany<T, TIntermediate, TResult>(this
            MaybeX<IEnumerable<T>> @this,
            MaybeX<Func<IEnumerable<T>, MaybeX<IEnumerable<TIntermediate>>>> selector,
            MaybeX<Func<IEnumerable<T>, IEnumerable<TIntermediate>, IEnumerable<TResult>>> projector
        ) =>
            from e in @this
            from s in selector
            from p in projector
            from r in s(e)
            select p(e, r);
    }
}
