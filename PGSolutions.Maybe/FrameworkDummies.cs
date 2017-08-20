using System;

using PGSolutions;
using PGSolutions.Monads;

#region Dummy implementations standing for standard Framework methods/classes
namespace System {
    using static FormattableString;

    internal static class ThrowHelper {
        public static void ThrowInvalidOperationException(string description) {
            throw new InvalidOperationException(Invariant($"Invalid operation - {description}."));
        }
    }
    internal static class ExceptionResource {
        public static string InvalidOperation_NoValue = "Invalid operation - No value.";
    }
}
namespace System.Runtime.Versioning {
    internal sealed class NonVersionableAttribute : Attribute {
        public NonVersionableAttribute() : base() { ; }
    }
}
#endregion
