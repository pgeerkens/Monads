using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  class DependencyInjectionExample {
      public static void Main(String[] args) {
          var ret = BusinessLogic.Run(ConfigFactory.GetConfig());

          Console.WriteLine(ret);
          Console.ReadKey();
      }

      private static class ConfigFactory {
          /// <summary>What we are injecting into all our methods.</summary>
          /// <remarks>
          /// In this example we have
          /// authorization and a logging method. Likely you could use this for sql connections,
          /// transactions, auth credentials, a pool of resources, etc.
          /// </remarks>
          public static Config GetConfig() {
              return new Config() {
                  AuthInfo  = "'pgeerkens'",
                  LogMethod = (str => MyCustomLog(str)),
              };
          }

          private static void MyCustomLog(String str) { Console.WriteLine("! " + str + " !"); }

          /// <summary>What we are injecting into all our methods.</summary>
          public class Config : IConfig {
              public String         AuthInfo  { get; set; }
              public Action<String> LogMethod { get; set; }
          }
      }
  }
}
