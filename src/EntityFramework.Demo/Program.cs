using System;
using System.Linq;
using EntityFramework.Demo.Demos;
using EntityFramework.Demo.Model;
using EntityFramework.Demo.TphModel;
using EntityFramework.Demo.TptDemo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EntityFramework.Demo
{
	public class Program
	{
		public static void Main()
		{
			var loggerFactory = GetLoggerFactory();
			var logger = loggerFactory.CreateLogger<DemosBase>();

			// ExecuteDemoDbQueries(loggerFactory, logger);
			// ExecuteTphQueries(loggerFactory, logger);
			ExecuteTptQueries(loggerFactory, logger);
		}

		private static void ExecuteTphQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetTphContext(loggerFactory))
			{
				if (!ctx.People.Any())
					ctx.SeedData();

				logger.LogInformation("[TPH] Customers: {@customers}", ctx.Customers);
				logger.LogInformation("[TPH] Employees: {@employees}", ctx.Employees);
			}
		}

		private static void ExecuteTptQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetTptContext(loggerFactory))
			{
				if (!ctx.People.Any())
					ctx.SeedData();

				logger.LogInformation("[TPT] Customers: {@customers}", ctx.Customers);
				logger.LogInformation("[TPT] Employees: {@employees}", ctx.Employees);
			}
		}

		private static void ExecuteDemoDbQueries(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
		{
			using (var ctx = GetDemoContext(loggerFactory))
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

		private static DemoDbContext GetDemoContext(ILoggerFactory loggerFactory)
		{
			var builder = new DbContextOptionsBuilder<DemoDbContext>()
								.UseSqlServer("Server=(local);Database=Demo;Trusted_Connection=True;MultipleActiveResultSets=true")
								.UseLoggerFactory(loggerFactory);

			var ctx = new DemoDbContext(builder.Options);

			ctx.Database.EnsureCreated();

			return ctx;
		}

		private static TphDbContext GetTphContext(ILoggerFactory loggerFactory)
		{
			var builder = new DbContextOptionsBuilder<TphDbContext>()
								.UseSqlServer("Server=(local);Database=TphDemo;Trusted_Connection=True;MultipleActiveResultSets=true")
								.UseLoggerFactory(loggerFactory);

			var ctx = new TphDbContext(builder.Options);

			ctx.Database.EnsureCreated();

			return ctx;
		}

		private static TptDbContext GetTptContext(ILoggerFactory loggerFactory)
		{
			var builder = new DbContextOptionsBuilder<TptDbContext>()
								.UseSqlServer("Server=(local);Database=TptDemo;Trusted_Connection=True;MultipleActiveResultSets=true")
								.UseLoggerFactory(loggerFactory);

			var ctx = new TptDbContext(builder.Options);

			ctx.Database.EnsureCreated();

			return ctx;
		}
	}
}
