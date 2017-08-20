using System;

#region Dummy implementations standing for standard Framework methods/classes
namespace System {
    internal static class ThrowHelper {
        public static void ThrowInvalidOperationException(string description) {
            throw new InvalidOperationException("Invalid operation - No value.");
        }
    }
    internal static class ExceptionResource {
        public static string InvalidOperation_NoValue = "Invalid operation - No value.";
    }
}
namespace System.Runtime.Versioning {
    internal class NonVersionableAttribute : Attribute {
        public NonVersionableAttribute() : base() { ; }
    }
}
#endregion
