using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.Model
{
   public static class DbFunctionsExtensions
   {
      public static int MyDbFunction(this DbFunctions functions, object args)
      {
         throw new NotSupportedException();
      }
   }
}
