using System;
using System.Diagnostics.CodeAnalysis;

namespace PGSolutions.Monads.DIDemo {
    using static Console;

    class DependencyInjectionExample {
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        public static int Main() {
            WriteLine(BusinessLogic.Run(ConfigFactory.GetConfig1()));
            WriteLine("==================================================");
            ReadKey();

            WriteLine(BusinessLogic.Run(ConfigFactory.GetConfig2()));
            WriteLine("==================================================");
            ReadKey();
            return 0;
        }

        private static class ConfigFactory {
            /// <summary>What we are injecting into all our methods.</summary>
            /// <remarks>
            /// In this example we have
            /// authorization and a logging method. Likely you could use this for sql connections,
            /// transactions, auth credentials, a pool of resources, etc.
            /// </remarks>
            [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
            public static Config GetConfig1() {
                return new Config() {
                    AuthInfo  = "'pgeerkens'",
                    LogMethod = (str =>  WriteLine("! " + str + " !")),
                };
            }
            /// <summary>What we are injecting into all our methods.</summary>
            /// <remarks>
            /// In this example we have
            /// authorization and a logging method. Likely you could use this for sql connections,
            /// transactions, auth credentials, a pool of resources, etc.
            /// </remarks>
            [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
            public static Config GetConfig2() {
                return new Config() {
                    AuthInfo  = "'Pieter'",
                    LogMethod = (str => WriteLine("!?! " + str + " !?!")),
                };
            }

            //[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
            //private static void MyCustomLog(string str) { WriteLine("! " + str + " !"); }

            /// <summary>What we are injecting into all our methods.</summary>
            public class Config : IConfig {
                public string         AuthInfo  { get; set; }
                public Action<string> LogMethod { get; set; }
            }
        }
    }
}
