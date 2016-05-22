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

namespace System.Diagnostics.Contracts {
    /// <summary>Extension methods to enhance Code Contracts and integration with Code Analysis.</summary>
    [Pure]
    public static class ContractExtensions {
#if CUSTOM_CONTRACT_VALIDATION
        /// <summary>Throws <c>ArgumentNullException{name}</c> if <c>value</c> is null.</summary>
        /// <param name="value">Value to be tested.</param>
        /// <param name="name">Name of the parameter being tested, for use in the exception thrown.</param>
        [ContractArgumentValidator]  // Requires Assemble Mode = Custom Parameter Validation
        public static void ContractedNotNullEx<T>([ValidatedNotNull]this T value, string name) {
              if (value == null) throw new ArgumentNullException(name);
              Contract.EndContractBlock();
        }
        /// <summary>Throws <c>ArgumentNullException{name}</c> if <c>value</c> is null.</summary>
        /// <param name="value">Value to be tested.</param>
        /// <param name="name">Name of the parameter being tested, for use in the exception thrown.</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        [ContractArgumentValidator]  // Requires Assemble Mode = Custom Parameter Validation
        public static void ContractedNotNullEx<T,E>([ValidatedNotNull]this T value, string name)
        where E:Exception {
            Contract.Requires<E>(value != null, name);
        }
#else
        /// <summary>Throws <c>ContractException{name}</c> if <c>value</c> is null.</summary>
        /// <param name="value">Value to be tested.</param>
        /// <param name="name">Name of the parameter being tested, for use in the exception thrown.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "name")]
        [ContractAbbreviator] // Requires Assemble Mode = Standard Contract Requires
        public static void ContractedNotNull<T>([ValidatedNotNull]this T value, string name) {
            Contract.Requires(value != null, name);
        }
#endif

        /// <summary>Decorator for an object which is to have it's object invariants assumed.</summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "t")]
        public static void AssumeInvariant<T>(this T t) { }

        /// <summary>Asserts the 'truth' of the logical implication <paramref name="condition"/> => <paramref name="contract"/>.</summary>
        /// <remarks>
        /// Doesn't work anymore in VS 2015 and CC 1.10.10126.4.
        /// Unfortunate, but not critical.
        /// </remarks>
        public static bool Implies(this bool condition, bool contract) {
            Contract.Ensures((! condition || contract)  ==  Contract.Result<bool>() );
            return ! condition || contract;
        }

        /// <summary>Returns true exactly if lower &lt;= value &lt; lower+height</summary>
        /// <param name="value">Vlaue being tested.</param>
        /// <param name="lower">Inclusive lower bound for the range.</param>
        /// <param name="height">Height of the range.</param>
        public static bool InRange(this int value, int lower, int height) {
            Contract.Ensures( (lower <= value && value < lower+height)  ==  Contract.Result<bool>() );
            return lower <= value && value < lower+height;
        }
    }

    /// <summary>Decorator for an incoming parameter that is contractually enforced as NotNull.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ValidatedNotNullAttribute : global::System.Attribute {}
}
