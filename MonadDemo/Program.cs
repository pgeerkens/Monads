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

using PGSolutions.Utilities.Monads;
using System.Diagnostics.CodeAnalysis;

namespace PGSolutions.Utilities.Monads.Demos {
#pragma warning disable CA1303

    static class Program {
    static readonly CultureInfo  _culture = CultureInfo.InvariantCulture;

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)")]
        static void Main() {
        BasicTest();

        WesDyerTest();

        MikeHadlowTest();

        ExternalStateTest();

        GcdTest1(new List<int>() {24,60,42,48,9}.AsReadOnly());

        var start = new GcdStart(40,1024);
  #if true
        Gcd_S4.Best.Run(start).ToMaybe().SelectMany( result => {
            var value   = result.Value;
            var title = Gcd_S4.GetTitle(Gcd_S4.Best.Run);
            Console.WriteLine("    GCD = {0} for {1} - {2}", value.Gcd, start, title);
            Console.WriteLine();
            return Maybe.Unit;
        } );
  #else
        var result8 = Gcd_S4.Best.Run(start);
        if (result8 != null) {
            var value   = result8.Value;
            var title = Gcd_S4.GetTitle(Gcd_S4.Best.Run);
            Console.WriteLine("    GCD = {0} for {1} - {2}",value.Gcd, start, title);
            Console.WriteLine();
        }
  #endif

        Console.ReadLine();
    }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        static void BasicTest() {
        var data = new List<Maybe<String>>() { "Fred", "George", null, "Ron", null };
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
      Console.WriteLine();
      {
        Console.WriteLine( ( from x in 5.ToMaybe()
                             from y in 7.ToMaybe()
                             select x + y ).ToNothingString() );

        Console.WriteLine( ( from x in 5.ToMaybe()
                             from y in Maybe<int>.Nothing
                             select x + y
                           ).ToNothingString() );

                Console.WriteLine(5.ToMaybe().SelectMany(x => Maybe<int>.Nothing, (x, y) => new { x, y })
                                             .Select(z => z.x + z.y )
                                             .ToNothingString() );
      }
      Console.WriteLine("_______________________");
    }

        // after Mike Hadlow: http://mikehadlow.blogspot.ca/2011/01/monads-in-c-5-maybe.html
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        static void MikeHadlowTest() {
      Console.WriteLine();
      { int denominator = 2;
        Console.WriteLine( ( from a in "Hello World!".ToMaybe()
                             from b in 12.DoSomeDivision(denominator)
                             from c in DateTime.Now.ToMaybe()
                             let sds = c.ToShortDateString()
                             select String.Format(_culture, "{0} {1} {2}", a, b,sds)
                           ).ToNothingString() );

        denominator = 0;
        Console.WriteLine( ( from a in "Hello World!".ToMaybe()
                             from b in 12.DoSomeDivision(denominator)
                             from c in DateTime.Now.ToMaybe()
                             let sds = c.ToShortDateString()
                             select String.Format(_culture, "{0} {1} {2}", a, b,sds)
                           ).ToNothingString() );
      }
      Console.WriteLine("_______________________");
    }

    public static string ToNothingString<T>(this Maybe<T> @this) {
      Contract.Ensures(Contract.Result<string>() != null);
      return @this.SelectMany<string>(v => v.ToString()) | "Nothing";
    }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private static void ExternalStateTest() {
      Console.WriteLine();
      var state = new ExternalState();
      var x = ( from a in (Maybe<Func<int>>)(state.GetState)
                select a
              ).Extract();
      var y = x();

      for (int i = 0; i++ < 5; ) state.GetState();
      Console.WriteLine(String.Format(_culture, "{0}", y ));
      Console.WriteLine(String.Format(_culture, "{0}", state.GetState()));
      Console.WriteLine(String.Format(_culture, "{0}", x() ));

      Console.WriteLine("_______________________");

      //var xx = new Maybe<IList<int>>(null);
      //Console.WriteLine(xx);
    }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)")]
        private static void GcdTest1(IList<int> list) {
        list.ContractedNotNull("null");
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
        list.ContractedNotNull("list");

        var result = new System.Text.StringBuilder("(");
        if (list.Any()) {
          result.Append(list[0]);
          for(var i=1; i<list.Count; i++) 
            result.Append(", " + list[i]);
        }
        return result.Append(")").ToString();
    }

    //private static void StateMaybeTest() {
    //  //Func<char,StateMaybe<bool,MyString> mbcasewst = 
    //  //    x => s =>  

    //  //Func<char,char,char,char,StateMaybe<bool,MyString>> x = (MaybeX,y,z,ec) =>
    //  //  x.b


    //  //Console.WriteLine("_______________________");
    //}

    private class ExternalState {
      private  int        _state;

      public ExternalState() { _state = -1;  }
      public  int GetState() { return ++_state; }
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
