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
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
    /// <summary>Extension methods for MaybeX&lt;T> to support LINQ "Comprehension" and "Fluent" syntax.</summary>
    /// <remarks>
    /// Unoptimized implementations of both Select() and SelectMany() have been retained
    /// as comments for documentation purposes, to emphasize the evolution to the
    /// optimized forms currently in use.
    /// 
    /// The intent is also to use this class as a tutorial for the exposition on
    /// generating the ptimized forms from the standard Monad implementations.
    /// </remarks>
    public static class MaybeXExtensions {
        /// <summary>LINQ-compatible implementation of Map as Select.</summary>
        [Pure]
        public static MaybeX<TResult>   Select<T, TResult>(
            this MaybeX<T> @this,
            Func<T, TResult> projector
        ) where T : class where TResult : class {
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            return @this.Map(projector);
        }

        /// <summary>LINQ-compatible implementation of Bind as SelectMany.</summary>
        [Pure]
        public static MaybeX<TResult>   SelectMany<TValue, TResult>(
            this MaybeX<TValue> @this,
            Func<TValue, MaybeX<TResult>> selector
        ) where TValue : class where TResult : class {
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            return @this.Bind(selector);
        }

        /// <summary>LINQ-compatible implementation of Flatten as SelectMany.</summary>
        [Pure]
        public static MaybeX<TResult>   SelectMany<TValue, T, TResult>(
            this MaybeX<TValue> @this,
            Func<TValue, MaybeX<T>> selector,
            Func<TValue, T, TResult> projector
        ) where T : class where TValue : class where TResult : class {
            selector.ContractedNotNull("selector");
            projector.ContractedNotNull("projector");
            Contract.Ensures(Contract.Result<MaybeX<TResult>>() != null);

            return @this.Flatten(selector,projector);
        }
    }
}
