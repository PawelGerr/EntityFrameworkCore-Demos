using System.Globalization;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Demo.CodeGeneration
{
   public class DesignTimeServices : IDesignTimeServices
   {
      /// <inheritdoc />
      public void ConfigureDesignTimeServices(IServiceCollection services)
      {
         services.AddSingleton<IPluralizer, Pluralizer>()
                 .AddSingleton(provider => new Inflector.Inflector(CultureInfo.CreateSpecificCulture("en-US")))
                 .AddSingleton<ICandidateNamingService, CustomCandidateNamingService>();
      }
   }
}
