using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  /// <summary>TODO</summary>
  /// <typeparam name="T"></typeparam>
  public delegate T IO<out T>();

  /// <summary>TODO</summary>
  /// <remarks>
  /// This pretty much comes straight from Dixin's Blog:
  ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
  /// except for all the Contract verification and some code reformatting.
  /// </remarks>
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces"), Pure]
  public static partial class IO {
    // η: T -> IO<T>
    /// <summary>TODO</summary>
    public static IO<T>                   ToIO<T>(this T value) {
      Contract.Ensures(Contract.Result<IO<T>>() != null);
      return () => value;
    }

    /// <summary>TODO</summary>
    public static readonly IO<int>    ConsoleRead     = new Func<int>(Console.Read).AsIO();
    /// <summary>TODO</summary>
    public static readonly IO<string> ConsoleReadLine = new Func<string>(Console.ReadLine).AsIO();

    /// <summary>TODO</summary>
    public static IO<ConsoleKeyInfo>  ConsoleReadKey()
    { Contract.Ensures(Contract.Result<IO<ConsoleKeyInfo>>() != null); return ()=>Console.ReadKey(); }

    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite(string value)
    {
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.Write(value);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite
      (object arg)
    {
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.Write(arg);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite<T>
      (string format, T arg)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.Write(arg);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite
      (string format, object arg1, object arg2)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.Write(format, arg1, arg2);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite
      (string format, object arg1, object arg2, object arg3)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.Write(format, arg1, arg2, arg3);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWrite
      (string format, params object[] arg)
    { 
      format.ContractedNotNull("format");
      arg.ContractedNotNull("arg");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null); 

      Console.Write(format, arg); 
      return () => Unit._;
    }

    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine
      ()
    {
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.WriteLine();
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine
      (string value)
    {
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.WriteLine(value);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine<T>
      (string format, T arg)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.WriteLine(arg);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine
      (string format, object arg1, object arg2)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.WriteLine(format, arg1, arg2);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine
      (string format, object arg1, object arg2, object arg3)
    {
      format.ContractedNotNull("format");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null);

      Console.WriteLine(format, arg1, arg2, arg3);
      return () => Unit._;
    }
    /// <summary>TODO</summary>
    public static IO<Unit>        ConsoleWriteLine
      (string format, params object[] arg)
    { 
      format.ContractedNotNull("format");
      arg.ContractedNotNull("arg");
      Contract.Ensures(Contract.Result<IO<Unit>>() != null); 

      Console.WriteLine(format, arg); 
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
