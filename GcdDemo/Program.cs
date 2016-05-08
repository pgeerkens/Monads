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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads.Demos {
    class Program {
        const string prompt = "Type 'Q' to quit; <Enter> to repeat ... ";

        //static int Main() { return true ? ComprehensionSyntax() : FluentSyntax(); }
        static int Main() { return ComprehensionSyntax(); }
        private static int ComprehensionSyntax() {
            return
                ( from list in  ( from pass in Enumerable.Range(0,int.MaxValue)
                                  let counter = Readers.Counter(0)
                                  select from state in gcdStates
                                         where predicate(pass,counter())
                                         select state
                                )
                  where ( from A in Gcd.Run(list.ToList())
                          from B in IO2.IO.ConsoleWrite(prompt)
                          from c in IO2.IO.ConsoleReadKey()
                          from D in IO2.IO.ConsoleWriteLine()
                          select Char.ToUpper(c.KeyChar) == 'Q' 
                        ).Invoke()
                  select 0
                ).FirstOrDefault(); // Doesn't assume result list non-empty, which is assumed by: ).First();
        }


        private static int FluentSyntax() {
            return
                ( Enumerable.Range(0,int.MaxValue)
                            .Select(pass => new {pass, counter = Readers.Counter(0)})
                            .Select(_    => gcdStates.Where(state => predicate(_.pass,_.counter()))
                                                     .Select(state => state)
                                   )
                ).Where(list => 
                   ( Gcd.Run(list.ToList())
                        .SelectMany(_ => IO2.IO.ConsoleWrite(prompt),(_,__) => new {_,__})
                        .SelectMany(_ => IO2.IO.ConsoleReadKey(),    (_,__) => new {_,c=__})
                        .SelectMany(_ => IO2.IO.ConsoleWriteLine(),  (_,__) => Char.ToUpper(_.c.KeyChar) == 'Q')
                   ).Invoke()
                ).Select(list => 0
                ).FirstOrDefault(); // Doesn't assume result list non-empty, unlike: ).First();
        }

        static readonly Func<int, int, bool> predicate =
                            (passNo, i) => passNo == 0  ?  i < 13
                                                        :  i < 2 || 11 < i;
        #region GCD States
        static readonly IList<GcdStart> gcdStates = new List<GcdStart>() {
            new GcdStart(         40,            40),           //  0
            new GcdStart(        1024,           40),           //  1

            new GcdStart(           9,           81),           //  2
            new GcdStart(           5,        32765),           //  3
            new GcdStart(           2,       199999),           //  4
            new GcdStart(           2, short.MaxValue*40 - 1),  //  5
            new GcdStart(        6553,        32765),           //  6
            new GcdStart(     - 32765,         6553),           //  7
            new GcdStart(       32765,        -6553),           //  8
            new GcdStart(       32768, int.MinValue),           //  9
            new GcdStart(int.MinValue,        32768),           // 10
            new GcdStart(int.MinValue, int.MinValue),           // 11

            new GcdStart(      -32767, int.MaxValue-1),         // 12
            new GcdStart(           2, short.MaxValue*1000 - 1) // 13
        };
        #endregion
    }
}
