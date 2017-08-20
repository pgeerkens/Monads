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
using System.Globalization;

using Xunit;

namespace PGSolutions.Monads {

    [ExcludeFromCodeCoverage]
    public class ReaderTests {
        [Fact]
        public static void ExecutionTest() {
            var isExecuted1 = false;
            var isExecuted2 = false;
            var isExecuted3 = false;
            var isExecuted4 = false;

            var culture = CultureInfo.InvariantCulture;

            Reader<int, int>               f1 = x => { isExecuted1 = true; return x + 1; };
            Reader<int, string>            f2 = x => { isExecuted2 = true; return x.ToString(culture); };
            Func<string, Reader<int, int>> f3 = x => y => { isExecuted3 = true; return x.Length + y; };
            Func<int, Func<int, int>>      f4 = x => y => { isExecuted4 = true; return x + y; };
            var query = from x in f1
                        from y in f2
                        from z in f3(y)
                        from _ in f1
                        let f4x = f4(x)
                        select f4x(z);
            Assert.False(isExecuted1,"Laziness: isExecuted1");
            Assert.False(isExecuted2,"Laziness: isExecuted2");
            Assert.False(isExecuted3,"Laziness: isExecuted3");
            Assert.False(isExecuted4,"Laziness: isExecuted4");
            Assert.Equal(1 + 1 + 1 + 1, query(1));//,"Equality");
            Assert.True(isExecuted1,"Execution: isExecuted1");
            Assert.True(isExecuted2,"Execution: isExecuted2");
            Assert.True(isExecuted3,"Execution: isExecuted3");
            Assert.True(isExecuted4,"Execution: isExecuted4");
        }
    }

    public class MonadLawTests {
        static Reader<Tuple<bool, string>, int> M = c => 2 + c.Item2.Length;
        static Func<int, Reader<Tuple<bool, string>, int>> addOne = x => c => x + 1;
        static Func<int, Reader<Tuple<bool, string>, int>> timesLength = x => c => x * c.Item2.Length;

        static Tuple<bool, string> config = Tuple.Create(true, "abc");

        /// <summary>Monad law 1: m.Monad().Bind(f) == f(m)</summary>
        [Fact]
        public static void MonadLaws() {
            Reader<Tuple<bool, string>, int> left = 1.ToReader<Tuple<bool, string>, int>().Bind(addOne);
            Reader<Tuple<bool, string>, int> right = addOne(1);
            //Contract.Assume(right != null);
            Assert.NotNull(right);
            Assert.Equal(left(config), right(config));
        }

        /// <summary>Monad law 2: M.Bind(Monad) == M</summary>
        [Fact]
        public static void MonadLaw2() {
            Tuple<bool, string> config = Tuple.Create(true, "abc");

            var expected = M.Bind(Reader.ToReader<Tuple<bool, string>, int>);
            var received = M;
            Assert.Equal(expected(config), received(config));
        }

        /// <summary>Monad law 3: M.Bind(f).Bind(g) == M.Bind(x => f(x).Bind(g))</summary>
        [Fact]
        public static void MonadLaw3() {
            var expected = M.Bind(addOne).Bind(timesLength);
            var received = M.Bind(x => addOne(x).Bind(timesLength));
            Assert.Equal(expected(config), received(config));
        }
    }

    public class TomAndJerryDependencyInjectionTest {
        [Fact]
        public static void TomAndJerryTest() {
            const string CrLf = "\r\n";
            const string expected = @"
Who is this? This is Tom!
Who is this? This is Jerry!";

            Reader<string, string> tom = env => env + "This is Tom!";
            Reader<string, string> jerry = env => env + "This is Jerry!";
            var query    = ( from t in tom
                             from j in jerry
                             select CrLf + t + CrLf + j
                           );

            var received = query("Who is this? ");
            Assert.Equal(expected, received);
        }
    }
}
