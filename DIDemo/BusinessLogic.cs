using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PGSolutions.Monads.DIDemo {
    using static CultureInfo;
    using static String;

    public interface IConfig {
        string         AuthInfo  { get; }
        Action<string> LogMethod { get; }
    }

    public static class BusinessLogic {
        /// <summary> Demonstrate nested composition through the magic of SelectMany</summary>
        public static string Run<TConfig>(TConfig config) where TConfig:IConfig {
            return (from intDb        in BusinessLogic<TConfig>.GetIntFromDB
                    from netstr       in BusinessLogic<TConfig>.GetStringFromNetwork
                    from writeSuccess in BusinessLogic<TConfig>.WriteStuffToDisk(intDb, netstr)
                    select writeSuccess ? Format(InvariantCulture,$"\nWe wrote\n   {intDb}\nand\n   {netstr}\nto disk")
                                        : "\nerror!"
            ) (config);
        }
    }

    internal static class BusinessLogic<TConfig> where TConfig:IConfig {
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object)")]
        public static Reader<TConfig,int>      GetIntFromDB { get {
            return new Reader<TConfig, int>(config => {
                config.LogMethod(Format(InvariantCulture,
                        $"Getting an int from DB using credentials {config.AuthInfo}"));
                        // ....
                return 4;
            } );
        } }

        public static Reader<TConfig, string> GetStringFromNetwork { get {
                return (from aDbInt in GetIntFromDB
                        from aGuid in GetGuidWithAuth
                            // ....
                        select (aDbInt + "|" + aGuid + "|")
                        );
            } }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        public static Reader<TConfig,bool>     WriteStuffToDisk(int i, string s) {
            return new Reader<TConfig,bool>(config => {
                config.LogMethod(Format(InvariantCulture,
                    $"writing\n   {i}\n   {s}\nto disk with credentials: {config.AuthInfo}"));
                    // ....
                return true;
            } );
        }

        private static Reader<TConfig,Guid>     GetGuidWithAuth { get {
            return new Reader<TConfig,Guid>(config => {
                config.LogMethod("Getting a GUID");
                return Guid.NewGuid();
            } );
        } }
    }
}
