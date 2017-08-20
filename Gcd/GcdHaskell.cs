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
    using StateBool = State<GcdStart, bool>;
    using StateRes = State<GcdStart, GcdResult>;

    using static State;
    using static StateExtensions;
    using static StateTransform;
    using static GcdS4;

    /// <summary>TODO</summary>
    internal static class GcdHaskell
    {
        #region Older Haskell implementations
        /* from http://mvanier.livejournal.com/5846.html
            gcd_s3 :: State GCDState Int
            gcd_s3 = 
              do (x, y) <- get
                 case compare x y of
                   GT -> do put (x-y,  y )
                            gcd_s3
                   LT -> do put ( x,  y-x)
                            gcd_s3
                   EQ -> return x

            run_gcd_s3 :: Int -> Int -> Int
            run_gcd_s3 x y = fst (runState gcd_s3 (x, y))
        */

        private static readonly StateBool GcdBody = new StateBool(s => NewPayload(s, s.A != s.B));
        // ~ 4.1 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel (iterative).")]
        public static StateRes Run1 => GetResult(
            GetCompose<GcdStart>(s =>
                          s.A > s.B ? Put(new GcdStart(s.A - s.B, s.B))
                        : s.A < s.B ? Put(new GcdStart(s.A, s.B - s.A))
                                    : Put(s)).Then(GcdBody)
                                             .DoWhile().Then(GcdExtract));

        // ~ 2.6 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel w/ Modify() instead of Get.Compose(s => Put(transform(s))).")]
        public static StateRes Run2 => GetResult(AlgorithmTransform.Modify().Then(GcdBody)
                                                                   .DoWhile().Then(GcdExtract));
        #endregion

        // ~ 1.5 sec
        /// <summary>TODO</summary>
        [Description("Straight Haskel w/ DoWhile().")]
        public static StateRes Run3 => GetResult(AlgorithMonad.DoWhile().Then(GcdExtract));
    }
}
