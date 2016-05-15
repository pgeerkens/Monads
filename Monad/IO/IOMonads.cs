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
using System.IO;

using static System.Console;

namespace PGSolutions.Utilities.Monads {
    /// <summary>TODO</summary>
    /// <remarks>
    /// Adapted from Dixin's Blog:
    ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
    /// by the addition of Contract verification and some code reformatting.
    /// </remarks>
    public static partial class IOMonads {
        private const string nullFormat = "format is null";

        // η: T -> IO<T>
        /// <summary>TODO</summary>
        public static IO<T> ToIO<T>(this T value) => new IO<T>(() => value);

        /// <summary>TODO</summary>
        public static IO<Unit> ReturnIOUnit(Action action) {
            action.ContractedNotNull("action");

            action();
            return Unit._.ToIO();
        }

        /// <summary>TODO</summary>
        public static readonly IO<int> ConsoleRead = new Func<int>(Read).AsIO();

        /// <summary>TODO</summary>
        public static readonly IO<string> ConsoleReadLine = new Func<string>(ReadLine).AsIO();

        /// <summary>TODO</summary>
        public static IO<ConsoleKeyInfo> ConsoleReadKey() => new IO<ConsoleKeyInfo>(() => ReadKey());

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(string value) =>
            ReturnIOUnit(() => Write(value));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(object arg) =>
            ReturnIOUnit(() => Write(arg));

        /// <summary>TODO</summary>
    //    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String,System.Object)")]
        public static IO<Unit> ConsoleWrite<T>(string format, T arg) =>
             ReturnIOUnit(() => Write(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String,System.Object,System.Object)")]
        public static IO<Unit> ConsoleWrite(string format, object arg1, object arg2) =>
             ReturnIOUnit(() => Write(format ?? nullFormat, arg1, arg2));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String,System.Object,System.Object,System.Object)")]
        public static IO<Unit> ConsoleWrite(string format, object arg1, object arg2, object arg3) =>
             ReturnIOUnit(() => Write(format ?? nullFormat, arg1, arg2, arg3));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String,System.Object[])")]
        public static IO<Unit> ConsoleWrite(string format, params object[] arg) =>
            ReturnIOUnit(() => Write(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine() =>
            ReturnIOUnit(() => WriteLine());

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine(string value) =>
            ReturnIOUnit(() => WriteLine(value));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object)")]
        public static IO<Unit> ConsoleWriteLine<T>(string format, T arg) =>
            ReturnIOUnit(() => WriteLine(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)")]
        public static IO<Unit> ConsoleWriteLine(string format, object arg1, object arg2) =>
             ReturnIOUnit(() => WriteLine(format ?? nullFormat, arg1, arg2));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)")]
        public static IO<Unit> ConsoleWriteLine(string format, object arg1, object arg2, object arg3) =>
             ReturnIOUnit(() => WriteLine(format ?? nullFormat, arg1, arg2, arg3));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object[])")]
        public static IO<Unit> ConsoleWriteLine(string format, params object[] arg) =>
            ReturnIOUnit(() => WriteLine(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification ="Delegates ARE immutable.")]
        public static readonly Func<string, IO<bool>> FileExists =
            new Func<string, bool>(File.Exists).AsIO();

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Delegates ARE immutable.")]
        public static readonly Func<string, IO<string>> FileReadAllText =
            new Func<string, string>(File.ReadAllText).AsIO();

        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Delegates ARE immutable.")]
        public static readonly Func<string, string, IO<Unit>> FileWriteAllText =
            new Action<string, string>(File.WriteAllText).AsIO();
    }
}
