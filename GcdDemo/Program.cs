﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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
using System.Globalization;
using System.Linq;

namespace PGSolutions.Monads.Demos {
    using static Console;
    using static CultureInfo;
    using static IOMonads;
    using static FormattableString;
    using static Syntax;

    enum Syntax { Imperative, Fluent, Query, Unknown }
    class Program {
        static string Prompt(string mode) =>
            Invariant($"{mode}: Type 'Q' to quit; <Enter> to repeat ... ");
        static readonly IList<GcdStart> gcdStartStates = new List<GcdStart>() {
             new GcdStart(         40,            40)            //  0
            ,new GcdStart(        1024,           40)            //  1

            ,new GcdStart(           9,           81)            //  2
            ,new GcdStart(           5,        32765)            //  3
            ,new GcdStart(           2,       199999)            //  4
            ,new GcdStart(           2, short.MaxValue*40 - 1)   //  5
            ,new GcdStart(        6553,        32765)            //  6
            ,new GcdStart(     - 32765,         6553)            //  7
            ,new GcdStart(       32765,        -6553)            //  8
            ,new GcdStart(       32768, int.MinValue)            //  9
            ,new GcdStart(int.MinValue,        32768)            // 10
            ,new GcdStart(int.MinValue, int.MinValue)            // 11

            ,new GcdStart(      -32767, int.MaxValue-1)          // 12
            ,new GcdStart(           2, short.MaxValue*1000 - 1) // 13
        };

        static bool _predicate(int passNo, int i) =>
                    passNo == 0  ?  i < 13
                                 :  i < 2 || 11 < i;

        static Syntax syntax = Imperative;
        static int Main() => ( syntax==Imperative ? ImperativeSyntax("Imperative Syntax")
                             : syntax==Fluent     ? FluentSyntax("Fluent (Method) Syntax")
                             : syntax==Query      ? QuerySyntax("Query (Comprehension) Syntax")
                                                  : new List<int> { 0 }
                             ).FirstOrDefault(); // Doesn't assume result list non-empty

        static IEnumerable<int> ImperativeSyntax(string mode) {
            for(int pass=0; pass < int.MaxValue; pass++) {
                var counter = 0;
                var enumerable = gcdStartStates.Where(state => _predicate(pass, counter++));

                Gcd.Run(enumerable.ToList());

                Write(Prompt(mode));
                var c = ReadKey();
                WriteLine();

                if (c.KeyChar.ToUpper(InvariantCulture) == 'Q') yield return 0;
            }
        }
        static IEnumerable<int> FluentSyntax(string mode) =>
            ( Enumerable.Range(0,int.MaxValue)
                        .Select(pass => new {pass, counter = Readers.Counter(0)})
                        .Select(_    => gcdStartStates.Where (state => _predicate(_.pass,_.counter()))
                                                      .Select(state => state)
                               )
            ).Where(enumerable => 
               ( (Gcd.Run(enumerable.ToList()) ).ToIO()
                    .SelectMany(_ => ConsoleWrite(Prompt(mode)),(_,__) => new {})
                    .SelectMany(_ => ConsoleReadKey(),          (_, c) => new {c})
                    .SelectMany(_ => ConsoleWriteLine(),        (_,__) => _.c.KeyChar.ToUpper(InvariantCulture) == 'Q')
               ).Invoke()
            ).Select(list => 0);
        static IEnumerable<int> QuerySyntax(string mode) =>
            from pass  in Enumerable.Range(0, int.MaxValue)
            let counter = Readers.Counter(0)
            select ( from state in gcdStartStates
                     where _predicate(pass, counter())
                     select state
            ) into enumerable
            where (from _   in Gcd.Run(enumerable.ToList()).ToIO()
                   from __  in ConsoleWrite(Prompt(mode))
                   from c   in ConsoleReadKey()
                   from ___ in ConsoleWriteLine()
                   select c.KeyChar.ToUpper(InvariantCulture) == 'Q'
                  ).Invoke()
            select 0;
    }

    public static class CharExtensions {
        public static char ToLower(this char c, CultureInfo culture) => char.ToUpper(c,culture);
        public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c,culture);
    }
}
