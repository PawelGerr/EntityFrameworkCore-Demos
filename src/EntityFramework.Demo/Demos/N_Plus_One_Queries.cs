using System;
using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming
namespace EntityFramework.Demo.Demos
{
   public class N_Plus_One_Queries : DemosBase
   {
      public N_Plus_One_Queries(DemoDbContext ctx, ILogger<N_Plus_One_Queries> logger)
         : base(ctx, logger)
      {
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p].[Id], [p].[Name], [p].[RowVersion], [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion]
      /// FROM [ProductGroups] AS [p]
      /// LEFT JOIN [Products] AS [p0] ON [p].[Id] = [p0].[GroupId]
      /// WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// ORDER BY [p].[Id], [p0].[Id]
      /// </summary>
      public void FetchGroups_Include_Products()
      {
         var groups = Context.ProductGroups
                             .Include(g => g.Products)
                             .Where(g => g.Name.Contains("Group"))
                             .ToList();

         Print("Fetched product groups", groups);
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p].[Id], [p].[Name], [p].[RowVersion], [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion]
      /// FROM [ProductGroups] AS [p]
      /// LEFT JOIN [Products] AS [p0] ON [p].[Id] = [p0].[GroupId]
      /// WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// ORDER BY [p].[Id], [p0].[Id]
      /// </summary>
      public void FetchGroups_Select_All_Products()
      {
         var groups = Context.ProductGroups
                             .Where(g => g.Name.Contains("Group"))
                             .Select(g => new
                                          {
                                             ProductGroup = g,
                                             g.Products
                                          })
                             .ToList();

         Print("Fetched product groups", groups);
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p].[Id], [p].[Name], [p].[RowVersion], [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
      /// FROM [ProductGroups] AS [p]
      /// LEFT JOIN (
      ///    SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion]
      ///    FROM [Products] AS [p0]
      ///    WHERE CHARINDEX(N'1', [p0].[Name]) > 0
      /// ) AS [t] ON [p].[Id] = [t].[GroupId]
      /// WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// ORDER BY [p].[Id], [t].[Id]
      /// </summary>
      public void FetchGroups_Select_Filtered_Products_without_ToList()
      {
         var groups = Context.ProductGroups
                             .Where(g => g.Name.Contains("Group"))
                             .Select(g => new
                                          {
                                             ProductGroup = g,
                                             Products = g.Products.Where(p => p.Name.Contains("1"))
                                          })
                             .ToList();

         Print("Fetched product groups", groups); // 1st iteration over product groups
         Print("Fetched product groups", groups); // 2nd iteration over product groups
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p].[Id], [p].[Name], [p].[RowVersion], [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
      /// FROM [ProductGroups] AS [p]
      /// LEFT JOIN (
      ///    SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion]
      ///    FROM [Products] AS [p0]
      ///    WHERE CHARINDEX(N'1', [p0].[Name]) > 0
      /// ) AS [t] ON [p].[Id] = [t].[GroupId]
      /// WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// ORDER BY [p].[Id], [t].[Id]
      /// </summary>
      public void FetchGroups_Select_Filtered_Products_with_ToList()
      {
         var groups = Context.ProductGroups
                             .Where(g => g.Name.Contains("Group"))
                             .Select(g => new
                                          {
                                             ProductGroup = g,
                                             Products = g.Products.Where(p => p.Name.Contains("1")).ToList()
                                          })
                             .ToList();

         Print("Fetched product groups", groups); // 1st iteration over product groups
         Print("Fetched product groups", groups); // 2nd iteration over product groups
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p].[Id], [p].[Name], [p].[RowVersion], [t0].[Id], [t0].[GroupId], [t0].[Name], [t0].[RowVersion]
      /// FROM [ProductGroups] AS [p]
      /// LEFT JOIN (
      ///    SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
      ///    FROM (
      ///       SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion], ROW_NUMBER() OVER(PARTITION BY [p0].[GroupId] ORDER BY [p0].[Id]) AS [row]
      ///       FROM [Products] AS [p0]
      ///    ) AS [t]
      ///    WHERE [t].[row] <= 1
      /// ) AS [t0] ON [p].[Id] = [t0].[GroupId]
      /// WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// </summary>
      public void FetchGroups_Select_First_Product()
      {
         var groups = Context.ProductGroups
                             .Where(g => g.Name.Contains("Group"))
                             .Select(g => new
                                          {
                                             ProductGroup = g,
                                             Product = g.Products.FirstOrDefault()
                                          })
                             .ToList();

         Print("Fetched product groups", groups);
      }

      /// <summary>
      /// == Executes 1 query ==
      ///
      /// SELECT [p0].[Id], [p0].[Name], [p0].[RowVersion], [p].[Id], [p].[GroupId], [p].[Name], [p].[RowVersion]
      /// FROM [Products] AS [p]
      /// INNER JOIN [ProductGroups] AS [p0] ON [p].[GroupId] = [p0].[Id]
      /// WHERE CHARINDEX(N'1', [p].[Name]) > 0
      /// </summary>
      public void FetchProducts_With_ProductGroup()
      {
         var products = Context.Products
                               .Where(g => g.Name.Contains("1"))
                               .Select(p => new
                                            {
                                               ProductGroup = p.Group,
                                               Product = p
                                            })
                               .ToList();

         Print("Fetched products", products);
      }

      /// <summary>
      /// == Throws ==
      ///
      /// System.InvalidOperationException: Processing of the LINQ expression
      /// 'GroupJoin<ProductGroup, Product, Guid, <>f__AnonymousType3<ProductGroup, IEnumerable<Product>>>(
      ///       outer: Where<ProductGroup>(
      ///          source: DbSet<ProductGroup>,
      ///          predicate: (g) => g.Name.Contains("Group")),
      ///       inner: Where<Product>(
      ///          source: DbSet<Product>,
      ///          predicate: (i) => i.Name.Contains("1")),
      ///       outerKeySelector: (g) => g.Id,
      ///       innerKeySelector: (p) => p.GroupId,
      ///       resultSelector: (g, p) => new {
      ///          ProductGroup = g,
      ///          Products = p
      ///       })' by 'NavigationExpandingExpressionVisitor' failed.
      /// </summary>
      public void FetchGroups_Select_Filtered_Products_via_GroupJoin_Throws()
      {
         try
         {
            var productsQuery = Context.Products.Where(i => i.Name.Contains("1"));

            var groups = Context.ProductGroups
                                .Where(g => g.Name.Contains("Group"))
                                .GroupJoin(productsQuery, g => g.Id, p => p.GroupId, (g, p) => new
                                                                                               {
                                                                                                  ProductGroup = g,
                                                                                                  Products = p
                                                                                               })
                                .ToList();

            Print("Fetched product groups", groups);
         }
         catch (Exception ex)
         {
            Logger.LogError(ex, "Error when using GroupJoin");
         }
      }

      /// <summary>
      /// == Executes 2 queries ==
      ///
      /// a) 1 for product groups
      ///
      /// 	SELECT [t].[Id], [t].[GroupId], [t].[Name], [t].[RowVersion]
      /// 	FROM [ProductGroups] AS [p]
      /// 	INNER JOIN (
      /// 	   SELECT [p0].[Id], [p0].[GroupId], [p0].[Name], [p0].[RowVersion]
      /// 	   FROM [Products] AS [p0]
      /// 	   WHERE CHARINDEX(N'1', [p0].[Name]) > 0
      /// 	) AS [t] ON [p].[Id] = [t].[GroupId]
      /// 	WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      ///
      ///
      /// b) 1 for products
      ///
      /// 	SELECT [p].[Id], [p].[Name], [p].[RowVersion]
      /// 	FROM [ProductGroups] AS [p]
      /// 	WHERE CHARINDEX(N'Group', [p].[Name]) > 0
      /// </summary>
      public void FetchGroups_Select_Filtered_Products_via_Lookup()
      {
         var groupsQuery = Context.ProductGroups
                                  .Where(g => g.Name.Contains("Group"));

         var productsByGroupId = groupsQuery.SelectMany(g => g.Products.Where(i => i.Name.Contains("1")))
                                            .ToLookup(p => p.GroupId);

         var groups = groupsQuery
                      .Select(g => new
                                   {
                                      ProductGroup = g,
                                      Products = productsByGroupId[g.Id]
                                   })
                      .ToList();

         Print("Fetched product groups", groups);
         Print("Fetched product groups", groups);
      }
   }
}
