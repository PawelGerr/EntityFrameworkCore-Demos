using System;
using System.Linq;
using EntityFramework.Demo.Demos;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EntityFramework.Demo
{
	public class Program
	{
		public static void Main()
		{
			var loggerFactory = GetLoggerFactory();
			var logger = loggerFactory.CreateLogger<DemosBase>();

			using (var ctx = GetContext(loggerFactory))
			{
				if (!ctx.Products.Any())
					ctx.SeedData();

				Execute_N_Plus_One_Queries_Demos(ctx, logger);
			}
		}

		private static void Execute_N_Plus_One_Queries_Demos(DemoDbContext ctx, ILogger<DemosBase> logger)
		{
			logger.LogInformation(" ==== {caption} ====", nameof(N_Plus_One_Queries));
			var demos = new N_Plus_One_Queries(ctx, logger);

			demos.FetchGroups_Include_Products();
			demos.FetchGroups_Select_All_Products();
			demos.FetchGroups_Select_Filtered_Products_without_ToList();
			demos.FetchGroups_Select_Filtered_Products_with_ToList();
			demos.FetchGroups_Select_First_Product();
			demos.FetchProducts_With_ProductGroup();

			demos.FetchGroups_Select_Filtered_Products_via_GroupJoin();
			demos.FetchGroups_Select_Filtered_Products_via_Lookup();
		}

		private static ILoggerFactory GetLoggerFactory()
		{
			var serilog = new LoggerConfiguration()
						.WriteTo.Console()
						.MinimumLevel.Verbose()
						.Destructure.AsScalar<Product>()
						.Destructure.AsScalar<ProductGroup>()
						.CreateLogger();

			return new LoggerFactory()
				.AddSerilog(serilog);
		}

		private static DemoDbContext GetContext(ILoggerFactory loggerFactory)
		{
			var builder = new DbContextOptionsBuilder<DemoDbContext>()
						.UseSqlServer("Server=(local);Database=Demo;Trusted_Connection=True;MultipleActiveResultSets=true")
						.UseLoggerFactory(loggerFactory);

			var ctx = new DemoDbContext(builder.Options);

			ctx.Database.EnsureCreated();

			return ctx;
		}
	}
}
