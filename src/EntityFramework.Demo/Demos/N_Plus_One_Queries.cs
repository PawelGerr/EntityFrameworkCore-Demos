using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming
namespace EntityFramework.Demo.Demos
{
	public class N_Plus_One_Queries : DemosBase
	{
		public N_Plus_One_Queries(DemoDbContext ctx, ILogger<DemosBase> logger)
			: base(ctx, logger)
		{
		}

		/// <summary>
		/// == Executes 2 queries ==
		///
		/// a) 1 query for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		/// 	ORDER BY
		/// 		[g].[Id]
		///
		/// b) 1 query for products
		///
		/// 	SELECT
		/// 		[g.Products].[Id], [g.Products].[GroupId], [g.Products].[Name]
		/// 	FROM
		/// 		[Products] AS [g.Products]
		/// 		INNER JOIN
		/// 		(
		/// 			SELECT
		/// 				[g0].[Id]
		/// 			FROM
		/// 				[ProductGroups] AS [g0]
		/// 			WHERE
		/// 				CHARINDEX(N'Group', [g0].[Name]) > 0
		/// 		) AS [t]
		/// 			ON [ g.Products].[GroupId] = [t].[Id]
		/// 	ORDER BY
		/// 		[t].[Id]
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
		/// == Executes 2 queries ==
		///
		/// a) 1 query for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		/// 	ORDER BY
		/// 		[g].[Id]
		///
		/// b) 1 query for products
		///
		/// 	SELECT
		/// 		[g.Products].[Id], [g.Products].[GroupId], [g.Products].[Name]
		/// 	FROM
		/// 		[Products] AS [g.Products]
		/// 		INNER JOIN
		/// 		(
		/// 			SELECT
		/// 				[g0].[Id]
		/// 			FROM
		/// 				[ProductGroups] AS [g0]
		/// 			WHERE
		/// 				CHARINDEX(N'Group', [g0].[Name]) > 0
		/// 		) AS [t]
		/// 			ON [ g.Products].[GroupId] = [t].[Id]
		/// 	ORDER BY
		/// 		[t].[Id]
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
		/// == Executes 11 queries ==
		///
		/// a) 1 query for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		///
		/// b) 5*x queries for products (i.e. 1 query per fetched product group and per iteration over product groups)
		/// 	Example: 5 product groups and 2 iterations over product groups => 1+(5*2) = 11 queries
		///
		/// 	SELECT
		/// 		[p].[Id], [p].[GroupId], [p].[Name]
		/// 	FROM
		/// 		[Products] AS [p]
		/// 	WHERE
		/// 		(CHARINDEX(N''1'', [p].[Name]) > 0) AND
		/// 		(@_outer_Id = [p].[GroupId])
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
		/// == Executes 2 queries ==
		///
		/// a) 1 query for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		///
		/// b) 1 query for products
		///	SELECT
		/// 		[g.Products].[Id], [g.Products].[GroupId], [g.Products].[Name], [t].[Id]
		/// 	FROM
		/// 		[Products] AS [g.Products]
		/// 		INNER JOIN
		/// 		(
		/// 			SELECT
		/// 				[g0].[Id]
		/// 			FROM
		/// 				[ProductGroups] AS[g0]
		/// 			WHERE
		/// 				CHARINDEX(N'Group', [g0].[Name]) > 0
		/// 		) AS [t]
		/// 			ON [g.Products].[GroupId] = [t].[Id]
		/// 	WHERE
		/// 		CHARINDEX(N'1', [g.Products].[Name]) > 0
		/// 	ORDER BY
		/// 		[t].[Id]
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
		/// == Executes 6 queries ==
		///
		/// a) 1 query for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		///
		/// b) 5 queries for first product (i.e. 1 query per fetched product group)
		///
		/// 	SELECT TOP(1)
		/// 		[p].[Id], [p].[GroupId], [p].[Name]
		/// 	FROM
		/// 		[Products] AS [p]
		/// 	WHERE
		/// 		@_outer_Id =  [p].[GroupId]
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
		/// SELECT
		/// 	[g].[Id], [g].[GroupId], [g].[Name], [g.Group].[Id], [g.Group].[Name]
		/// FROM
		/// 	[Products] AS [g]
		/// 	INNER JOIN
		/// 		[ProductGroups] AS [g.Group]
		/// 		ON [g].[GroupId] = [g.Group].[Id]
		/// WHERE
		/// 	CHARINDEX(N'1', [g].[Name]) > 0
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
		/// == Executes 1 query ==
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name], [t].[Id], [t].[GroupId], [t].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 		LEFT JOIN
		/// 		(
		/// 			SELECT
		/// 				[i].[Id], [i].[GroupId], [i].[Name]
		/// 			FROM
		/// 				[Products] AS [i]
		/// 			WHERE
		/// 				CHARINDEX(N'1', [i].[Name]) > 0
		/// 		) AS [t]
		/// 			ON [g].[Id] = [t].[GroupId]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		/// 	ORDER BY
		/// 		[g].[Id]
		/// </summary>
		public void FetchGroups_Select_Filtered_Products_via_GroupJoin()
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

		/// <summary>
		/// == Executes 2 queries ==
		///
		/// a) 1 for product groups
		///
		/// 	SELECT
		/// 		[g].[Id], [g].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 	WHERE
		/// 		CHARINDEX(N'Group', [g].[Name]) > 0
		///
		/// b) 1 for products
		/// 	SELECT
		/// 		[g.Products].[Id], [g.Products].[GroupId], [g.Products].[Name]
		/// 	FROM
		/// 		[ProductGroups] AS [g]
		/// 		INNER JOIN
		/// 			[Products] AS [g.Products]
		/// 			ON [g].[Id] = [g.Products].[GroupId]
		/// 	WHERE
		/// 		(CHARINDEX(N'Group', [g].[Name]) > 0) AND
		/// 		(CHARINDEX(N'1', [g.Products].[Name]) > 0)
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
