using System;

namespace PGSolutions.Monads.DIDemo {
    using static Console;
    using static FormattableString;

    class DependencyInjectionExample {
        public static int Main() {
            WriteLine(BusinessLogic.Run(ConfigFactory.GetConfig1()));
            WriteLine(Invariant($"=================================================="));
            ReadKey();

            WriteLine(BusinessLogic.Run(ConfigFactory.GetConfig2()));
            WriteLine(Invariant($"=================================================="));
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
            public static IConfig GetConfig1()
                => new Config("'pgeerkens'", str => WriteLine(Invariant($"! {str} !")));

            /// <summary>What we are injecting into all our methods.</summary>
            /// <remarks>
            /// In this example we have
            /// authorization and a logging method. Likely you could use this for sql connections,
            /// transactions, auth credentials, a pool of resources, etc.
            /// </remarks>
            public static IConfig GetConfig2()
                => new Config("'Pieter'", str => WriteLine(Invariant($"!?! {str} !?!")));

            /// <summary>What we are injecting into all our methods.</summary>
            public class Config : IConfig {
                public Config(string authInfo, Action<string> logMethod) {
                    AuthInfo = authInfo; LogMethod = logMethod;    
                }
                public string         AuthInfo  { get; }
                public Action<string> LogMethod { get; }
            }
        }
    }
}
