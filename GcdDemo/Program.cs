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
using System.Linq;

namespace PGSolutions.Utilities.Monads.Demos {
    using static Char;
    using static Console;
    using static IOMonads;

    class Program {
        static string Prompt(string mode) => 
            String.Format("{0}: Type 'Q' to quit; <Enter> to repeat ... ",mode);
        #region GCD States
        static readonly IList<GcdStart> gcdStartStates = new List<GcdStart>() {
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

        static bool _predicate(int passNo, int i) =>
                    passNo == 0  ?  i < 13
                                 :  i < 2 || 11 < i;

        static int i = 0;
        static int Main() => ( i==0 ? ImperativeSyntax("Imperative")
                             : i==1 ? FluentSyntax("Fluent")
                             : i==2 ? ComprehensionSyntax("Comprehension")
                                    : ComprehensionSyntax2("Comprehension2")
                             ).FirstOrDefault(); // Doesn't assume result list non-empty, unlike: ).First();

        static IEnumerable<int> ImperativeSyntax(string mode) {
            for(int pass=0; pass < int.MaxValue; pass++) {
                var counter = 0;
                var list = gcdStartStates.Where(state => _predicate(pass, counter++));

                Gcd.Run2((Maybe<IList<GcdStart>>)(list.ToList()));
                Write(Prompt(mode));
                var c = ReadKey();
                WriteLine();

                if (ToUpper(c.KeyChar) == 'Q') yield return 0; //break;
            }
      //      return 0;
        }
        #region Fluent syntax
        static IEnumerable<int> FluentSyntax(string mode) =>
            ( Enumerable.Range(0,int.MaxValue)
                        .Select(pass => new {pass, counter = Readers.Counter(0)})
                        .Select(_    => gcdStartStates.Where(state => _predicate(_.pass,_.counter()))
                                                      .Select(state => state)
                               )
            ).Where(list => 
               ( Gcd.Run(list.ToList())
                    .SelectMany(_ => ConsoleWrite(Prompt(mode)),(_,__) => new {})
                    .SelectMany(_ => ConsoleReadKey(),          (_, c) => new {c})
                    .SelectMany(_ => ConsoleWriteLine(),        (_,__) => ToUpper(_.c.KeyChar) == 'Q')
               ).Invoke()
            ).Select(list => 0
            );
        #endregion
        #region Comprehension syntax
        static IEnumerable<int> ComprehensionSyntax(string mode) =>
            ( from list in  ( from pass in Enumerable.Range(0,int.MaxValue)
                              let counter = Readers.Counter(0)
                              select from state in gcdStartStates
                                     where _predicate(pass,counter())
                                     select state
                            )
              where ( from _   in Gcd.Run(list.ToList())
                      from __  in ConsoleWrite(Prompt(mode))
                      from c   in ConsoleReadKey()
                      from ___ in ConsoleWriteLine()
                      select ToUpper(c.KeyChar) == 'Q' 
                    ).Invoke()
              select 0
            );
        #endregion
        #region Comprehension syntax w/ Maybe<T>
        static IEnumerable<int> ComprehensionSyntax2(string mode) =>
            from pass in Enumerable.Range(0, int.MaxValue)
            let counter = Readers.Counter(0)
            select ( from state in gcdStartStates
                     where _predicate(pass, counter())
                     select state ) 
            into list
            where ( from _ in Gcd.Run2((Maybe<IList<GcdStart>>)(list.ToList())) | IO<Unit>.Empty
                    from __  in ConsoleWrite(Prompt(mode))
                    from c   in ConsoleReadKey()
                    from ___ in ConsoleWriteLine()
                    select ToUpper(c.KeyChar) == 'Q'
                  ).Invoke()
            select 0;
        #endregion
    }
}
