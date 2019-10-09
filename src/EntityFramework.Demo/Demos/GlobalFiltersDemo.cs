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
         /**
          * Generated query:
          *
          * [Parameters=[@__ef_filter__p_0='True', @__ef_filter___localeFilter_1='']
          *
          * SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion], [t0].[ProductId], [t0].[Locale], [t0].[Description]
          * FROM (
          *    SELECT TOP(1) [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
          *    FROM [Products] AS [p]
          *    ORDER BY [p].[Name]
          * ) AS [t]
          * LEFT JOIN (
          *    SELECT [p0].[ProductId], [p0].[Locale], [p0].[Description]
          *    FROM [ProductTranslation] AS [p0]
          *    WHERE (@__ef_filter__p_0 = CAST(1 AS bit)) OR (([p0].[Locale] = @__ef_filter___localeFilter_1) AND @__ef_filter___localeFilter_1 IS NOT NULL)
          * ) AS [t0] ON [t].[Id] = [t0].[ProductId]
          * ORDER BY [t].[Name], [t].[Id], [t0].[ProductId], [t0].[Locale]
          */

         var product = Context.Products
                              .Include(p => p.Translations)
                              .OrderBy(p => p.Name)
                              .First();

         Logger.LogInformation("Found product: {@product}", product);
      }

      public void LoadTranslationsWithLocaleFilter([NotNull] string locale)
      {
         /**
          * Generated query:
          *
          * Parameters=[@__ef_filter__p_0='False', @__ef_filter___localeFilter_1='en']
          *
          * SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion], [t0].[ProductId], [t0].[Locale], [t0].[Description]
          * FROM (
          *    SELECT TOP(1) [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
          *    FROM [Products] AS [p]
          *    ORDER BY [p].[Name]
          * ) AS [t]
          * LEFT JOIN (
          *    SELECT [p0].[ProductId], [p0].[Locale], [p0].[Description]
          *    FROM [ProductTranslation] AS [p0]
          *    WHERE (@__ef_filter__p_0 = CAST(1 AS bit)) OR (([p0].[Locale] = @__ef_filter___localeFilter_1) AND @__ef_filter___localeFilter_1 IS NOT NULL)
          * ) AS [t0] ON [t].[Id] = [t0].[ProductId]
          * ORDER BY [t].[Name], [t].[Id], [t0].[ProductId], [t0].[Locale]
          */

         using var _ = Context.SetTranslationFilter(locale);

         var product = Context.Products
                              .Include(p => p.Translations)
                              .OrderBy(p => p.Name)
                              .First();

         Logger.LogInformation("Found product: {@product}", product);
      }
   }
}
