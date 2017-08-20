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
using System.Threading;
using System.Threading.Tasks;

namespace PGSolutions.Monads {
    /// <summary>TODO</summary>
    public interface ICancellableTask {
        /// <summary>TODO</summary>
        CancellationToken CancellationToken { get; }
    }

    /// <summary>TODO</summary>
    public abstract class FsmTask<TEnv> where TEnv : ICancellableTask {
        /// <summary>TODO</summary>
        public delegate FsmReader           FsmState();

        /// <summary>TODO</summary>
        public delegate Task<FsmState>      FsmReader(TEnv e);

        /// <summary>TODO</summary>
        public delegate FsmReader           FsmTransition(TEnv e);

        /// <summary>TODO</summary>
        public async Task<Unit>             Run (TEnv e) {
            try {
                var state = await Start(e)(e);
                while (state != null)  state = await state()(e);
            } catch (OperationCanceledException) { ; }
            return Unit._;
        }

        /// <summary>TODO</summary>
        protected abstract FsmTransition    Start { get; }
    }

    /// <summary>TODO</summary>
    public static class FsmExtensions {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static FsmTask<TEnv>.FsmReader  GetState<TEnv>(this FsmTask<TEnv>.FsmTransition @this,
            int delayMilliseconds,
            IO<Unit> action
        ) where TEnv:ICancellableTask =>
            e => from _ in ( from _ in action
                             select Task.Delay(delayMilliseconds,e.CancellationToken).ToTaskUnit()
                           ).Invoke()
                 select new FsmTask<TEnv>.FsmState(() => @this(e));
    }
}
