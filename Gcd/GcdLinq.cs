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
using System.ComponentModel;
using System.Linq;

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos {
    using StateBool = State<GcdStart, bool>;
    using StateInt = State<GcdStart, int>;
    using StateRes = State<GcdStart, GcdResult>;

    using static State;
    using static StateExtensions;
    using static StateTransform;
    using static GcdS4;

    /// <summary>TODO</summary>
    internal static class GcdLinq {
        #region Older LINQ implementations
        // ~ 3.5 sec
        /// <summary>TODO</summary>
        [Description("Basic LINQ w/ Enumerate(Transform<TState>, TState) and using l'let'.")]
        public static StateRes Run1 => GetResult(new StateBool(start =>
                      (from payload in AlgorithMonad.Enumerate(start)
                       where !payload.Item2
                       select payload
                      ).First()).Then(GcdExtract));

        // ~ 2.3 sec
        /// <summary>TODO</summary>
        [Description("Better LINQ w/ Enumerate(State<TState,TValue>, TState).")]
        public static StateRes Run2 => GetResult(new StateBool(start =>
                      (from payload in AlgorithMonad.Enumerate(start)
                       where !payload.Item2
                       select payload
                      ).First()).Then(GcdExtract));
        #endregion

        // ~ 1.5 sec
        /// <summary>TODO</summary>
        [Description("Best LINQ w/ Enumerate(Transform<TState>, TState) and w/o using 'let'.")]
        public static StateRes Run3 => GetResult(new StateInt(start =>
                      (from s in AlgorithmTransform.Enumerate(start)
                       where s.A == s.B
                       select NewPayload(s, s.A)
                      ).First()));
    }
}
