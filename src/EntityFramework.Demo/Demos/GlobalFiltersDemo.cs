using System;
using System.Linq;
using EntityFramework.Demo.Model;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class GlobalFiltersDemo : DemosBase
   {
      public GlobalFiltersDemo([NotNull] DemoDbContext ctx, [NotNull] ILogger<GlobalFiltersDemo> logger)
         : base(ctx, logger)
      {
         Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      }

      public void LoadTranslationsWithoutFilter()
      {
         var product = Context.Products
                              .Include(p => p.Translations)
                              .OrderBy(p => p.Name)
                              .First();

         Logger.LogInformation("Found product: {@product}", product);
      }

      public void LoadTranslationsWithLocaleFilter([NotNull] string locale)
      {
         using var _ = Context.SetTranslationFilter(locale);

         var product = Context.Products
                              .Include(p => p.Translations)
                              .OrderBy(p => p.Name)
                              .First();

         Logger.LogInformation("Found product: {@product}", product);
      }
   }
}
