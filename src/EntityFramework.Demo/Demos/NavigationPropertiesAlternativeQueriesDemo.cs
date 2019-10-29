using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Demo.Model;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class NavigationPropertiesAlternativeQueriesDemo : DemosBase
   {
      public NavigationPropertiesAlternativeQueriesDemo(DemoDbContext ctx, ILogger logger)
         : base(ctx, logger)
      {
      }

      public void UseNavigationalProperty()
      {
         // Generated query:
         //
         //    SELECT [p].[Id], [p].[Name], [p].[RowVersion], [t0].[Id], [t0].[GroupId], [t0].[Name], [t0].[RowVersion]
         //    FROM [ProductGroups] AS [p]
         //    LEFT JOIN (
         //        SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
         //        FROM (
         //            SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion], ROW_NUMBER() OVER(PARTITION BY [p0].[GroupId] ORDER BY [p0].[Name]) AS [row]
         //            FROM [Products] AS [p0]
         //        ) AS [t]
         //        WHERE [t].[row] <= 1
         //    ) AS [t0] ON [p].[Id] = [t0].[GroupId]
         var groups = Context.ProductGroups
                             .Select(g => new
                                          {
                                             Group = g,
                                             FirstProduct = g.Products.OrderBy(p => p.Name).FirstOrDefault()
                                          })
                             .ToList();

         Print("Loaded using navigational property", groups);
      }

      public void WithoutNavigationalProperty()
      {
         // Generated query is the same as above:
         //
         //    SELECT [p].[Id], [p].[Name], [p].[RowVersion], [t0].[Id], [t0].[GroupId], [t0].[Name], [t0].[RowVersion]
         //    FROM [ProductGroups] AS [p]
         //    LEFT JOIN (
         //        SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
         //        FROM (
         //            SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion], ROW_NUMBER() OVER(PARTITION BY [p0].[GroupId] ORDER BY [p0].[Name]) AS [row]
         //            FROM [Products] AS [p0]
         //        ) AS [t]
         //        WHERE [t].[row] <= 1
         //    ) AS [t0] ON [p].[Id] = [t0].[GroupId]
         var groups = Context.ProductGroups
                             .Select(g => new
                                          {
                                             Group = g,
                                             FirstProduct = Context.Products.OrderBy(p => p.Name).FirstOrDefault(p => p.GroupId == g.Id)
                                          })
                             .ToList();

         Print("Loaded without using navigational property", groups);
      }
   }
}
