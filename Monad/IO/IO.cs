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
using System.IO;

using static System.Diagnostics.Contracts.Contract;
using static System.Console;

#if Undefined
namespace PGSolutions.Utilities.Monads {
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    public delegate T IO<out T>();

    /// <summary>TODO</summary>
    /// <remarks>
    /// Adapted from Dixin's Blog:
    ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
    /// by the addition of Contract verification and some code reformatting.
    /// </remarks>
    public static partial class IO {
        // η: T -> IO<T>
        /// <summary>TODO</summary>
        public static IO<T>               ToIO<T>(this T value) {
            Ensures(Contract.Result<IO<T>>() != null);
            return () => value;
        }

        /// <summary>TODO</summary>
        public static readonly IO<int>    ConsoleRead     = new Func<int>(Console.Read).AsIO();
        /// <summary>TODO</summary>
        public static readonly IO<string> ConsoleReadLine = new Func<string>(Console.ReadLine).AsIO();

        /// <summary>TODO</summary>
        public static IO<ConsoleKeyInfo>  ConsoleReadKey() {
            Ensures(Contract.Result<IO<ConsoleKeyInfo>>() != null);
            return ()=>Console.ReadKey();
        }

        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite(string value) {
            Ensures(Contract.Result<IO<Unit>>() != null);

            Write(value);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite(object arg) {
            Ensures(Contract.Result<IO<Unit>>() != null);

            Write(arg);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite<T>(string format, T arg) {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            Write(arg);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite(string format, object arg1, object arg2) 
        {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            Write(format, arg1, arg2);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite
            (string format, object arg1, object arg2, object arg3)
        {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            Write(format, arg1, arg2, arg3);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWrite(string format, params object[] arg) { 
            format.ContractedNotNull("format");
            arg.ContractedNotNull("arg");
            Ensures(Contract.Result<IO<Unit>>() != null); 

            Write(format, arg); 
            return () => Unit._;
        }

        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine() {
            Ensures(Contract.Result<IO<Unit>>() != null);

            WriteLine();
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine
            (string value)
        {
            Ensures(Contract.Result<IO<Unit>>() != null);

            WriteLine(value);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine<T>
            (string format, T arg)
        {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            WriteLine(arg);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine
            (string format, object arg1, object arg2)
        {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            WriteLine(format, arg1, arg2);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine
            (string format, object arg1, object arg2, object arg3)
        {
            format.ContractedNotNull("format");
            Ensures(Contract.Result<IO<Unit>>() != null);

            WriteLine(format, arg1, arg2, arg3);
            return () => Unit._;
        }
        /// <summary>TODO</summary>
        public static IO<Unit>            ConsoleWriteLine
            (string format, params object[] arg)
        { 
            format.ContractedNotNull("format");
            arg.ContractedNotNull("arg");
            Ensures(Contract.Result<IO<Unit>>() != null); 

            WriteLine(format, arg); 
            return () => Unit._;
        }

        /// <summary>TODO</summary>
        public static readonly Func<string, IO<bool>>         FileExists        = new Func<string, bool>(File.Exists).AsIO();
        /// <summary>TODO</summary>
        public static readonly Func<string, IO<string>>       FileReadAllText   = new Func<string, string>(File.ReadAllText).AsIO();
        /// <summary>TODO</summary>
        public static readonly Func<string, string, IO<Unit>> FileWriteAllText  = new Action<string, string>(File.WriteAllText).AsIO();
    }
}
#endif

namespace PGSolutions.Utilities.Monads.IO2 {
    /// <summary>TODO</summary>
    /// <remarks>
    /// Adapted from Dixin's Blog:
    ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
    /// by the addition of Contract verification and some code reformatting.
    /// </remarks>
    public static partial class IO {
        private const string nullFormat = "format is null";

        // η: T -> IO<T>
        /// <summary>TODO</summary>
        public static IO<T> ToIO<T>(this T value) {
            return new IO2.IO<T>(() => value);
        }

        /// <summary>TODO</summary>
        public static readonly IO<int> ConsoleRead = new Func<int>(Console.Read).AsIO();
        /// <summary>TODO</summary>
        public static readonly IO<string> ConsoleReadLine = new Func<string>(Console.ReadLine).AsIO();

        /// <summary>TODO</summary>
        public static IO<ConsoleKeyInfo> ConsoleReadKey() {
            return new IO<ConsoleKeyInfo>(() => Console.ReadKey());
        }

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(string value) =>
            ReturnIoUnit(() => Write(value));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(object arg) =>
            ReturnIoUnit(() => Write(arg));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite<T>(string format, T arg) =>
             ReturnIoUnit(() => Write(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(string format, object arg1, object arg2) =>
             ReturnIoUnit(() => Write(format ?? nullFormat, arg1, arg2));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(string format, object arg1, object arg2, object arg3) =>
             ReturnIoUnit(() => Write(format ?? nullFormat, arg1, arg2, arg3));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWrite(string format, params object[] arg) =>
            ReturnIoUnit(() => Write(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine() =>
            ReturnIoUnit(() => WriteLine());

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine(string value) =>
            ReturnIoUnit(() => WriteLine(value));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine<T>(string format, T arg) =>
            ReturnIoUnit(() => WriteLine(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine(string format, object arg1, object arg2) =>
             ReturnIoUnit(() => WriteLine(format ?? nullFormat, arg1, arg2));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine(string format, object arg1, object arg2, object arg3) =>
             ReturnIoUnit(() => WriteLine(format ?? nullFormat, arg1, arg2, arg3));

        /// <summary>TODO</summary>
        public static IO<Unit> ConsoleWriteLine(string format, params object[] arg) =>
            ReturnIoUnit(() => WriteLine(format ?? nullFormat, arg));

        /// <summary>TODO</summary>
        public static readonly Func<string, IO<bool>> FileExists =
            new Func<string, bool>(File.Exists).AsIO();

        /// <summary>TODO</summary>
        public static readonly Func<string, IO<string>> FileReadAllText =
            new Func<string, string>(File.ReadAllText).AsIO();

        /// <summary>TODO</summary>
        public static readonly Func<string, string, IO<Unit>> FileWriteAllText =
            new Action<string, string>(File.WriteAllText).AsIO();

        private static IO<Unit> ReturnIoUnit(Action action) {
            action.ContractedNotNull("action");

            action();
            return new IO<Unit>(() => Unit._);
        }
    }
}
