using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PGSolutions.Monads {
    using static StringComparison;
#if ! true
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public delegate ValueTuple<T,string>?       Parser<T>(string input);

    /// <summary>TODO</summary>
    public static class Parser {
        private static ValueTuple<T,string>     ToResult<T>(T t, string s) => ValueTuple.Create(t,s);
        private static ValueTuple<T,string>?    ToMonad<T>(this ValueTuple<T,string> value) => value;
        private static ValueTuple<T,string>     Empty<T>() => ToResult(default(T),null);

        /// <summary>TODO</summary>
        public static string    AsString<T>(this ValueTuple<T,string>? @this, Func<T,string> writer) =>
#else
    /// <summary>TODO</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public delegate X<Tuple<T,string>>          Parser<T>(string input);

    /// <summary>TODO</summary>
    public static class Parser {
        private static Tuple<T,string>          ToResult<T>(T t, string s) => Tuple.Create(t,s);
        private static X<Tuple<T,string>>       ToMonad<T>(this Tuple<T,string> value) => value;
        private static Tuple<T,string>          Empty<T>() => ToResult(default(T),null);

        /// <summary>TODO</summary>
        public static string    AsString<T>(this X<Tuple<T,string>> @this, Func<T,string> writer) =>
#endif
            ( from p in @this from t in writer(p.Item1).AsX() select t ) | "Nothing";

        /// <summary>TODO</summary>
        public static Parser<T> ToParser<T>(this T value) => s => (ToResult(value, s));

        /// <summary>TODO</summary>
        public static Parser<TResult>   Select<TValue,TResult>(this Parser<TValue> @this,
            Func<TValue,TResult> projector
        ) => s => from p in @this(s) select ToResult(projector(p.Item1), p.Item2);

        /// <summary>TODO</summary>
        public static Parser<TResult>   SelectMany<TValue,TResult>(this Parser<TValue> @this,
            Func<TValue,Parser<TResult>> selector
        ) => s => from p in @this(s) from u in selector(p.Item1)(p.Item2) select u;

        /// <summary>TODO</summary>
        public static Parser<TResult>   SelectMany<TValue,T,TResult>(this Parser<TValue> @this,
            Func<TValue,Parser<T>> selector,
            Func<TValue,T,TResult> resultSelector
        ) => s => from t1 in @this(s)
                  from u in selector(t1.Item1)(t1.Item2)
                  select ToResult(resultSelector(t1.Item1,u.Item1), u.Item2);

        /// <summary>Returns a <see cref="Parser{T}"/> upon recognizing the string <paramref name="target"/>; else null.</summary>
        /// <typeparam name="T">The type of the state-object for the returned parser.</typeparam>
        /// <param name="target">The string to recognize.</param>
        /// <param name="acceptString">The work to be done upon recognizing <paramref name="target"/>.</param>
        public static Parser<T> Find<T>(this string target, 
            Func<string,T> acceptString
        ) => s => s.StartsWith(target,Ordinal) 
                ? ToResult(acceptString(target), s.Skip(target.Length).ToString()).ToMonad()
                : Empty<T>();

        /// <summary>Returns a <see cref="Parser{T}"/> upon recognizing the regular expression <paramref name="target"/>; else null.</summary>
        /// <typeparam name="T">The type of the state-object for the returned parser.</typeparam>
        /// <param name="target">The Regular Expression (<see cref="Regex"/>) to recognize.</param>
        /// <param name="acceptMatch">The work to be done upon recognizing <paramref name="target"/>.</param>
        public static Parser<T> Find<T>(this Regex target, 
            Func<Match,T> acceptMatch
        ) => s => { var match = target.Match(s);
                    return (match.Success) 
                        ? ToResult(acceptMatch(match), s.Skip(match.Length).ToString()).ToMonad()
                        : Empty<T>();
                  };

        /// <summary>Returns a <see cref="Parser{T}"/> upon recognizing the a string in <paramref name="targets"/>; else null.</summary>
        /// <typeparam name="T">The type of the state-object for the returned parser.</typeparam>
        /// <param name="targets">The collection of possible target strings to recognize.</param>
        /// <param name="acceptString">The work to be done upon recognizing an element from <paramref name="targets"/>.</param>
        public static Parser<T> Find<T>(this IEnumerable<string> targets, 
            Func<string,T> acceptString
        ) => s => ( from match in targets
                    where s.StartsWith(match,Ordinal)
                    select ToResult(acceptString(match), s.Skip(match.Length).ToString()).ToMonad()
                  ).FirstOrDefault();

        /// <summary>TODO</summary>
        public static Parser<TResult> FindAll<T,TResult> (this Parser<T> parser,
            Func<string,T>  acceptString,
            Func<T,TResult> acceptT
        ) => s => {
            var t1 = Empty<T>().ToMonad();
            var t2 = Empty<TResult>();
            while ((t1=parser(t2.Item2)).HasValue) {
                t2 = ( from v1 in t1
                       from v2 in ToResult(acceptString(v1.Item2),v1.Item2).ToMonad()
                       from v3 in ToResult(acceptT(v2.Item1),v2.Item2).ToMonad()
                       select v3
                     ) | Empty<TResult>();
                }
            return t2;
        };

        /// <summary>TODO</summary>
        public static Parser<TResult> FindAll2<T,TResult> (this Parser<T> parser,
            Func<string,T>  acceptString,
            Func<T,TResult> acceptT
        ) => s => {
            var t1 = Empty<T>().ToMonad();
            var t2 = Empty<TResult>();
            while ((t1=parser(t2.Item2)).HasValue) {
                #pragma warning disable CS0618 // silence complaints about use of BitwiseOr below
                t2 = (from v2 in ( from v1 in t1
                                   from v11 in ToResult(acceptString(v1.Item2), v1.Item2).ToMonad()
                                   select v11)
                      from v3 in ToResult(acceptT(v2.Item1), v2.Item2).ToMonad()
                      select v3
                     ) | (Empty<TResult>());
                #pragma warning restore CS0618
            }
            return t2;
        };
    }
}
