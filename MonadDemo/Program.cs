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

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos {
    using static String;
    using static FormattableString;
    using static Console;

    static class Program {
        static void Main() {
            BasicTest();

            WesDyerTest();

            ExternalStateTest();

            GcdTest1(new List<int>() {24,60,42,48,9}.AsReadOnly());

            var start = new GcdStart(40,1024);

            GcdS4.BestRun(start).ToNullable2().SelectMany(result => {
                var value = result.Item2;
                var title = GcdS4.GetTest("Best.Run");
                WriteLine(Invariant($"    GCD = {value.Gcd} for {start} - {title}"));
                WriteLine(Invariant($"_______________________"));
                WriteLine(Invariant($"Hit ENTER to close."));
                return Unit.Empty.ToNullable2();
            });

            ReadLine();
        }

        static void BasicTest() {
            var data = ( from e in new List<string>() { "Fred", "George", null, "Ron", null }
                         select e.AsX() ).ToList().AsReadOnly();
            WriteLine(Join("/", from e in data
                                select e.ToString() ) + "/" );

            WriteLine(Join("/", from e in data
                                select e.ToNothingString() ) + "/" );

            // Equivalent code in first "Fluent" and then "Comprehension" syntax:
            WriteLine(Join("/", from e in data
                                where e.HasValue
                                select e.ToString()
                            ) + "/" );

            WriteLine(Join("/", data.Where (e => e.HasValue)
                                            .Select(e => e.ToString())
                                         ) + "/" );
            WriteLine("_______________________");
        }

        // after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        static void WesDyerTest() {
            WriteLine();
            WriteLine( ( from x in 5.ToNullable()
                         from y in 7.ToNullable()
                         select x + y ).ToNothingString() );

            WriteLine( ( from x in 5.ToNullable()
                         from y in (int?)null
                         select x + y
                     ).ToNothingString() );

            WriteLine(5.ToNullable()
                       .SelectMany(x => (int?)null, (x, y) => x+y)
                       .ToNothingString() );
           
            WriteLine("_______________________");
        }

        private static void ExternalStateTest() {
            WriteLine();
            var state = new ExternalState();
            var x = (from a in state.GetState.AsX() select a) | (()=>-99);
            var y = x();

            for (int i = 0; i++ < 5; ) state.GetState();
            WriteLine(Invariant($"y:     {y} (Expect 1)"));
            WriteLine(Invariant($"state: {state.Value} (Expect 6)"));
            WriteLine(Invariant($"x():   {x()} (Expect 7)"));
            WriteLine("_______________________");
        }

        private static void GcdTest1(IList<int> list) {
            list.ContractedNotNull(nameof(list));
            if (list.Any()) {
                int gcd   = list[0];

                ( from n in list 
                  select gcd = GcdS4.BestRun(new GcdStart(n, gcd)).Item2.Gcd
                ).LastOrDefault();

                WriteLine(Invariant($"    GCD = {gcd} for {list.FormatList()??""}"));
                WriteLine();
            }
        }

        private static string FormatList<T>(this IList<T> list) =>
            list!=null ? "(" + Join(", ",list) + ")" : null;

        private class ExternalState {
            private  int        _state;

            public ExternalState() { _state = 0;  }
            public  Func<int> GetState => () => ++_state; 
            public int Value  => _state;
        }
    }
    internal struct MyString {
        public MyString(string s) { _value = s; }

        public string Value { get { return _value; } } readonly string _value;

        public static MyString operator +(MyString @this, string suffix) {
            return new MyString(@this.Value??"" + suffix??"");
        }
    }
}
