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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace PGSolutions.Monads {
    /// <summary>Extension methods to enhance Code Contracts and integration with Code Analysis.</summary>
    public static class ContractExtensions {
        /// <summary>Throws <c>ContractException{name}</c> if <c>value</c> is null.</summary>
        /// <param name="value">Value to be tested.</param>
        /// <param name="name">Name of the parameter being tested, for use in the exception thrown.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "name")]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ContractedNotNull<T>([ValidatedNotNull]this T value, string name) {
            Debug.Assert(value != null, name);
        }

        ///// <summary>Decorator for an object which is to have it's object invariants assumed.</summary>
        //[SuppressMessage("CodeCracker.Usage", "CC0057:UnusedParameters", MessageId = "t")]
        //[SuppressMessage("CodeCracker.Usage", "CC0090:MissingParametersInXmlDocs", MessageId = "t")]
        //[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "t")]
        //public static void AssumeInvariant<T>(this T t) { }
    }

    /// <summary>Decorator for an incoming parameter that is contractually enforced as NotNull.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ValidatedNotNullAttribute : Attribute {}
}
