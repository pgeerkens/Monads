using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {

  public interface IConfig {
      String         AuthInfo  { get; }
      Action<String> LogMethod { get; }
  }

  public static class BusinessLogic {
      const string successMessage = "\nWe wrote\n   {0}\nand\n   {1}\nto disk";
      const string errorMessage   = "\nerror!";

      /// <summary> Demonstrate nested composition through the magic of SelectMany</summary>
      public static string Run<TConfig>(TConfig config) where TConfig:IConfig {
          //var bl = new BusinessLogic<TConfig>();
          return (
              from intDb        in BusinessLogic<TConfig>.GetIntFromDB()
              from netstr       in BusinessLogic<TConfig>.GetStrFromNetwork()
              from writeSuccess in BusinessLogic<TConfig>.WriteStuffToDisk(intDb, netstr)
              select (writeSuccess ? successMessage.BuildMe(intDb,netstr)
                                   : errorMessage)
        ) (config);
      }
  }

  internal class BusinessLogic<TConfig> where TConfig:IConfig { 
      public BusinessLogic(TConfig config) { ; }

      public  static Reader<TConfig, int>     GetIntFromDB() {
          Contract.Ensures(Contract.Result<Reader<TConfig, int>>() != null);

          const string dbMessage = "Getting an int from DB using credentials {0}";
          return new Reader<TConfig, int>(config => {
              config.LogMethod(dbMessage.BuildMe(config.AuthInfo));
                        // ....
              return 4;
          } );
      }

      public  static Reader<TConfig, string>  GetStrFromNetwork() {
          Contract.Ensures(Contract.Result<Reader<TConfig, string>>() != null);

          return  ( from aDbInt in GetIntFromDB()
                    from aGuid in GetGuidWithAuth()
                        // ....
                    select (aDbInt + "|" + aGuid + "|")
                  );
      }               
    
      public  static Reader<TConfig, Boolean> WriteStuffToDisk(int i, string str) {
          Contract.Ensures(Contract.Result<Reader<TConfig, Boolean>>() != null);

          const string diskMessage = "writing\n   {1}\n   {2}\nto disk with credentials: {0}";
          return new Reader<TConfig, Boolean>(config => {
              config.LogMethod(diskMessage.BuildMe(config.AuthInfo, i, str));
                  // ....
              return true;
          } );
      }

      private static Reader<TConfig, Guid>   GetGuidWithAuth() {
          return new Reader<TConfig,Guid>(config => {
              config.LogMethod("Getting a GUID");
              return Guid.NewGuid();
          } );
      }
  }
}
