using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  /// <summary>TODO</summary>
  /// <remarks>
  /// This pretty much comes straight from Dixin's Blog:
  ///     https://weblogs.asp.net/dixin/category-theory-via-c-sharp-18-more-monad-io-monad
  /// except for all the Contract verification and some code reformatting.
  /// </remarks>
  [Pure]
    public static class IOLinqExtensions {
        // Select: (TSource -> TResult) -> (IO<TSource> -> IO<TResult>)
        /// <summary>TODO</summary>
        public static IO<TResult>             Select<TSource, TResult>(this
            IO<TSource> source,
            Func<TSource, TResult> selector
        ) {
            source.ContractedNotNull("source");
            selector.ContractedNotNull("selector");
            Contract.Ensures(Contract.Result<IO<TResult>>() != null);

            return source.SelectMany(item => selector(item).ToIO());
        }

        // Required by LINQ.
        /// <summary>TODO</summary>
        public static IO<TResult>             SelectMany<TSource, TSelector, TResult>(this
            IO<TSource> source, 
            Func<TSource, IO<TSelector>> selector, 
            Func<TSource, TSelector, TResult> resultSelector
        ) {
            source.ContractedNotNull("source");
            selector.ContractedNotNull("selector");
            resultSelector.ContractedNotNull("resultSelector");
            Contract.Ensures(Contract.Result<IO<TResult>>() != null);

            return () => { var sourceItem = source();
                            return resultSelector(sourceItem, selector(sourceItem)());
                        };
        }

            // Not required, just for convenience.
            /// <summary>TODO</summary>
            public static IO<TResult>             SelectMany<TSource, TResult>(this
                IO<TSource> source,
                Func<TSource, IO<TResult>> selector
            ) {
                source.ContractedNotNull("source");
                selector.ContractedNotNull("selector");
                Contract.Ensures(Contract.Result<IO<TResult>>() != null);

 //               return source.SelectMany(selector, Functions.Second);
                return () => selector(source())();
        }
    }
}
