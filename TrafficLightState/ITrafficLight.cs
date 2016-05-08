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
using System.Threading;

using PGSolutions.Utilities.Monads;

namespace TrafficLightDemo {
  /// <summary>TODO</summary>
  [ContractClass(typeof(ITrafficLightContract<>))]
  public interface ITrafficLight<T> {
    T Red    { get; }
    T Yellow { get; }
    T Green  { get; }

    ISettableLight<T> CrossTown        { get; }
    ISettableLight<T> UpTownLeftTurn   { get; }
    ISettableLight<T> DownTownLeftTurn { get; }
    ISettableLight<T> UpDownTown       { get; }

    CancellationToken CancellationToken { get; }
  }

  /// <summary>TODO</summary>
  [ContractClassFor(typeof(ITrafficLight<>))]
  public abstract class ITrafficLightContract<T> : ITrafficLight<T> {
    private ITrafficLightContract() { }

    public T Red    { [Pure]get {Contract.Ensures(Contract.Result<T>() != null); return default(T); } }
    public T Yellow { [Pure]get {Contract.Ensures(Contract.Result<T>() != null); return default(T); } }
    public T Green  { [Pure]get {Contract.Ensures(Contract.Result<T>() != null); return default(T); } }

    public ISettableLight<T> CrossTown        { [Pure]get {Contract.Ensures(Contract.Result<ISettableLight<T>>() != null); return null; } }
    public ISettableLight<T> UpTownLeftTurn   { [Pure]get {Contract.Ensures(Contract.Result<ISettableLight<T>>() != null); return null; } }
    public ISettableLight<T> DownTownLeftTurn { [Pure]get {Contract.Ensures(Contract.Result<ISettableLight<T>>() != null); return null; } }
    public ISettableLight<T> UpDownTown       { [Pure]get {Contract.Ensures(Contract.Result<ISettableLight<T>>() != null); return null; } }

    public CancellationToken CancellationToken { get { return default(CancellationToken); } }
  }
}
