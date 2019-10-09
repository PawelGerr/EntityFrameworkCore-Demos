using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.Model
{
   public static class DbFunctionsExtensions
   {
      public static int MyDbFunction(this DbFunctions functions, object args)
      {
         throw new NotSupportedException("Do not call this method directly!");
      }

      public static int MyDbFunction(this DbFunctions functions, object arg1, object arg2)
      {
         throw new NotSupportedException("Do not call this method directly!");
      }
   }
}
