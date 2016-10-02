using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PGSolutions.Monads.DIDemo {
    using static CultureInfo;
    using static String;
    using static System.Diagnostics.Contracts.Contract;

    public interface IConfig {
        string         AuthInfo  { get; }
        Action<string> LogMethod { get; }
    }

    public static class BusinessLogic {
        const string successMessage = "\nWe wrote\n   {0}\nand\n   {1}\nto disk";
        const string errorMessage   = "\nerror!";

        /// <summary> Demonstrate nested composition through the magic of SelectMany</summary>
        public static string Run<TConfig>(TConfig config) where TConfig:IConfig {
            return (from intDb        in BusinessLogic<TConfig>.GetIntFromDB
                    from netstr       in BusinessLogic<TConfig>.GetStringFromNetwork
                    from writeSuccess in BusinessLogic<TConfig>.WriteStuffToDisk(intDb, netstr)
                    select writeSuccess ? Format(InvariantCulture,successMessage,intDb,netstr)
                                        : errorMessage
            ) (config);
        }
    }

    public static class BusinessLogic<TConfig> where TConfig:IConfig {
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Reader<TConfig,int>      GetIntFromDB { get {
            const string dbMessage = "Getting an int from DB using credentials {0}";
            Ensures(Result<Reader<TConfig,int>>() != null);

            return new Reader<TConfig, int>(config => {
                config.LogMethod(Format(InvariantCulture,dbMessage,config.AuthInfo));
                        // ....
                return 4;
            } );
        } }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Reader<TConfig, string> GetStringFromNetwork { get {
                Ensures(Result<Reader<TConfig, string>>() != null);

                return (from aDbInt in GetIntFromDB
                        from aGuid in GetGuidWithAuth
                            // ....
                        select (aDbInt + "|" + aGuid + "|")
                        );
            } }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static Reader<TConfig,bool>     WriteStuffToDisk(int i, string s) {
            Ensures(Result<Reader<TConfig, bool>>() != null);

            const string diskMessage = "writing\n   {1}\n   {2}\nto disk with credentials: {0}";
            return new Reader<TConfig,bool>(config => {
                config.LogMethod(Format(InvariantCulture,diskMessage,config.AuthInfo, i, s));
                    // ....
                return true;
            } );
        }

        private static Reader<TConfig,Guid>     GetGuidWithAuth { get {
            Ensures(Result<Reader<TConfig,Guid>>() != null);

            return new Reader<TConfig,Guid>(config => {
                config.LogMethod("Getting a GUID");
                return Guid.NewGuid();
            } );
        } }
    }
}
