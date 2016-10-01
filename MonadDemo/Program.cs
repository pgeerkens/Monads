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
using System.Globalization;
using System.Linq;

using PGSolutions.Monads;
using System.Diagnostics.CodeAnalysis;

namespace PGSolutions.Monads.Demos {
#pragma warning disable CA1303
    static class Program {
        static readonly CultureInfo  _culture = CultureInfo.InvariantCulture;

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)")]
        static void Main() {
            BasicTest();

            WesDyerTest();

            ExternalStateTest();

            GcdTest1(new List<int>() {24,60,42,48,9}.AsReadOnly());

            var start = new GcdStart(40,1024);

            Gcd_S4.Best.Run(start).ToMaybe().SelectMany(result => {
                var value = result.Value;
                var title = Gcd_S4.GetTest("Best.Run");
                Console.WriteLine("    GCD = {0} for {1} - {2}", value.Gcd, start, title);
                Console.WriteLine("_______________________");
                Console.WriteLine("Hit ENTER to close.");
                return Unit._.ToMaybe();
            });

            Console.ReadLine();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        static void BasicTest() {
            //var data = new List<Maybe<String>>() { "Fred", "George", null, "Ron", null };
            var data = ( from e in new List<String>() { "Fred", "George", null, "Ron", null }
                         select e.ToMaybe() ).ToList().AsReadOnly();
            Console.WriteLine(string.Join("/", from e in data
                                               select e.ToString() ) + "/" );

            Console.WriteLine(string.Join("/", from e in data
                                               select e.ToNothingString() ) + "/" );

            // Equivalent code in first "Fluent" and then "Comprehension" syntax:
            Console.WriteLine(string.Join("/", from e in data
                                               where e.HasValue
                                               select e.ToString()
                                         ) + "/" );

            Console.WriteLine(string.Join("/", data.
                                                  Where (e => e.HasValue).
                                                  Select(e => e.ToString())
                                         ) + "/" );
            Console.WriteLine("_______________________");
        }

        // after Wes Dyer: http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        static void WesDyerTest() {
            Console.WriteLine(); {
                Console.WriteLine( ( from x in 5.ToNullable()
                                     from y in 7.ToNullable()
                                     select x + y ).ToNothingString() );

                Console.WriteLine( ( from x in 5.ToNullable()
                                     from y in default(int?)
                                     select x + y
                                   ).ToNothingString() );

                Console.WriteLine(5.ToNullable()
                                   .SelectMany(x => default(int?), (x, y) => x+y)
                                   .ToNothingString() );
            }
            Console.WriteLine("_______________________");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private static void ExternalStateTest() {
            Console.WriteLine();
            var state = new ExternalState();
            var x = (from a in state.GetState.AsMaybeX() select a) | (()=>-99);
            var y = x();

            for (int i = 0; i++ < 5; ) state.GetState();
            Console.WriteLine("y:     {0} (Expect 1)".FormatMe(_culture, y ));
            Console.WriteLine("state: {0} (Expect 6)".FormatMe(_culture, state.Value));
            Console.WriteLine("x():   {0} (Expect 7)".FormatMe(_culture, x() ));

            Console.WriteLine("_______________________");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)")]
        private static void GcdTest1(IList<int> list) {
            list.ContractedNotNull(nameof(list));
            if (list.Any()) {
                int gcd   = list[0];

                ( from n in list 
                  select gcd = Gcd_S4.Best.Run(new GcdStart(n, gcd)).Value.Gcd
                ).LastOrDefault();

                Console.WriteLine("    GCD = {0} for {1}", gcd, list.Format());
                Console.WriteLine();
            }
        }

        private static string Format<T>(this IList<T> list) {
            list.ContractedNotNull(nameof(list));

            var result = new System.Text.StringBuilder("(");
            if (list.Any()) {
                result.Append(list[0]);
                for(var i=1; i<list.Count; i++) 
                    result.Append(", " + list[i]);
            }
            return result.Append(")").ToString();
        }

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
#pragma warning restore CA1303
}
