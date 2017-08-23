#region Dummy implementations standing for standard Framework methods/classes
namespace System {
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
