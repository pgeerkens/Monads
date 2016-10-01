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
using System.Diagnostics.Contracts;
using System.Threading;

namespace PGSolutions.Monads.TrafficLightDemo {
    using static Contract;

    /// <summary>TODO</summary>
    [ContractClass(typeof(ITrafficLightContract<,>))]
    public interface ITrafficLight<TSettableLight,T> : ICancellableTask
            where TSettableLight:ISettableLight<T>
    {
        /// <summary>TODO</summary>
        T Red    { get; }
        /// <summary>TODO</summary>
        T Yellow { get; }
        /// <summary>TODO</summary>
        T Green  { get; }

        /// <summary>TODO</summary>
        TSettableLight CrossTown        { get; }
        /// <summary>TODO</summary>
        TSettableLight UpTownLeftTurn   { get; }
        /// <summary>TODO</summary>
        TSettableLight DownTownLeftTurn { get; }
        /// <summary>TODO</summary>
        TSettableLight UpDownTown       { get; }
    }

    /// <summary>Contract class for <see cref="ITrafficLight{T}"/>.</summary>
    [ContractClassFor(typeof(ITrafficLight<,>))]
    [Pure]
    public abstract class ITrafficLightContract<TSettableLight,T> : ITrafficLight<TSettableLight,T>
            where TSettableLight:ISettableLight<T>
    {
        private ITrafficLightContract() { }

        /// <inheritdoc/>
        public T Red    { get { Ensures(Result<T>() != null); return default(T); } }
        /// <inheritdoc/>
        public T Yellow { get { Ensures(Result<T>() != null); return default(T); } }
        /// <inheritdoc/>
        public T Green  { get { Ensures(Result<T>() != null); return default(T); } }

        /// <inheritdoc/>
        public TSettableLight CrossTown        { get { Ensures(Result<ISettableLight<T>>() != null); return default(TSettableLight); } }
        /// <inheritdoc/>
        public TSettableLight UpTownLeftTurn   { get { Ensures(Result<ISettableLight<T>>() != null); return default(TSettableLight); } }
        /// <inheritdoc/>
        public TSettableLight DownTownLeftTurn { get { Ensures(Result<ISettableLight<T>>() != null); return default(TSettableLight); } }
        /// <inheritdoc/>
        public TSettableLight UpDownTown       { get { Ensures(Result<ISettableLight<T>>() != null); return default(TSettableLight); } }

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get { return default(CancellationToken); } }
    }
}
