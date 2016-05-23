using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace PGSolutions.Utilities.Monads.StaticContracts {
    using static Contract;

    public struct MaybeAssume<T> {
        ///<summary>Create a new Maybe{T}.</summary>
        private MaybeAssume(T value) : this() {
            Ensures(!HasValue ||  _value != null);

            _value    = value;
            _hasValue = _value != null;
        }

        ///<summary>Returns whether this Maybe{T} has a value.</summary>
        public bool HasValue { get { return _hasValue; } }

        ///<summary>Extract value of the Maybe{T}, substituting <paramref name="defaultValue"/> as needed.</summary>
        [Pure]
        public T BitwiseOr(T defaultValue) {
            defaultValue.ContractedNotNull("defaultValue");
            Ensures(Result<T>() != null);

            var result = !_hasValue ? defaultValue : _value;
            //        Assume(result != null);
            return result;
        }

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Invariant(!HasValue || _value != null);
        }

        readonly T    _value;
        readonly bool _hasValue;
    }
}
