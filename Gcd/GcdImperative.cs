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

namespace PGSolutions.Monads.Demos {
    using StateInt = State<GcdStart, int>;
    using StateRes = State<GcdStart, GcdResult>;

    using static State;
    using static GcdS4;

    /// <summary>TODO</summary>
    internal static class GcdImperative {
        // ~ 0.14 sec
        /// <summary>TODO</summary>
        [Description("Fully imperative; w/ substitution.")]
        public static StateRes Run1 => GetResult(new StateInt(s => {
            s.ContractedNotNull(nameof(s));
            while(s.A != s.B) s = AlgorithmTransform(s);
            return NewPayload(s, s.A);
        }));

        // ~ 0.04 sec
        /// <summary>TODO</summary>
        [Description("Fully imperative; unrolled w/ substitution.")]
        public static StateRes Run2 => GetResult(new StateInt(s => {
            s.ContractedNotNull(nameof(s));
            while(s.A != s.B)
            {
                var x = s.A; var y = s.B;
                s = x > y ? new GcdStart(x-y, x)
                  : x < y ? new GcdStart(x, y-x)
                          : s;
            }
            return NewPayload(s, s.A);
        }));
    }
}
