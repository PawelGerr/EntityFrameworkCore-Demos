using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Demo.Model
{
   public class DemoDbContextOptionsExtension : IDbContextOptionsExtension
   {
      public DbContextOptionsExtensionInfo Info { get; }

      public DemoDbContextOptionsExtension()
      {
         Info = new DemoDbContextOptionsExtensionInfo(this);
      }

      public void ApplyServices(IServiceCollection services)
      {
         services.AddSingleton<IMethodCallTranslatorPlugin, MyMethodCallTranslatorPlugin>();
      }

      public void Validate(IDbContextOptions options)
      {
      }

      private class DemoDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
      {
         public override bool IsDatabaseProvider => false;
         public override string LogFragment => String.Empty;

         public DemoDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
         {
         }

         public override long GetServiceProviderHashCode()
         {
            return 0;
         }

         public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
         {
         }
      }
   }
}
