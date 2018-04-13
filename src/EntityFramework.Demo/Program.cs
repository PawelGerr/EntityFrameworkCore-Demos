using System;
using System.Linq;
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
			var logger = loggerFactory.CreateLogger<Demos>();

			using (var ctx = GetContext(loggerFactory))
			{
				if (!ctx.Products.Any())
					ctx.SeedData();

				ExecuteMultipleQueriesDemos(ctx, logger);
			}
		}

		private static void ExecuteMultipleQueriesDemos(DemoDbContext ctx, ILogger<Demos> logger)
		{
			logger.LogInformation(" ==== {caption} ====", nameof(MultipleQueriesDemos));
			var demos = new MultipleQueriesDemos(ctx, logger);

			// demos.FetchGroups_Include_Products();
			// demos.FetchGroups_Select_All_Products();
			// demos.FetchGroups_Select_5_Products_without_ToList();
			demos.FetchGroups_Select_5_Products_with_ToList();
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
