using System;
using System.Linq;
using EntityFramework.Demo.Demos;
using EntityFramework.Demo.Model;
using EntityFramework.Demo.TphModel.CodeFirst;
using EntityFramework.Demo.TptModel.CodeFirst;
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
			ExecuteTphQueries(loggerFactory, logger);
			ExecuteTptQueries(loggerFactory, logger);
		}

		private static void ExecuteDemoDbQueries(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
		{
			using (var ctx = GetDemoContext(loggerFactory))
			{
				if (!ctx.Products.Any())
					ctx.SeedData();

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
		}

		private static void ExecuteTphQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetTphContext(loggerFactory))
			{
				if (!ctx.People.Any())
					ctx.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(Tph_Queries));

				var demos = new Tph_Queries(ctx, logger);
				demos.FetchCustomers();
				demos.FetchEmployees();
			}
		}

		private static void ExecuteTptQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetTptContext(loggerFactory))
			{
				if (!ctx.People.Any())
					ctx.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(Tpt_Queries));

				var demos = new Tpt_Queries(ctx, logger);
				demos.FetchCustomers();
				demos.FetchEmployees();
			}
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
			return GetDbContext<DemoDbContext>(loggerFactory, "Demo", o => new DemoDbContext(o));
		}

		public static TphDbContext GetTphContext(ILoggerFactory loggerFactory)
		{
			return GetDbContext<TphDbContext>(loggerFactory, "TphDemo", o => new TphDbContext(o));
		}

		public static TptDbContext GetTptContext(ILoggerFactory loggerFactory)
		{
			return GetDbContext<TptDbContext>(loggerFactory, "TptDemo", o => new TptDbContext(o));
		}

		private static T GetDbContext<T>(ILoggerFactory loggerFactory, string dbName, Func<DbContextOptions<T>, T> ctxFactory)
			where T : DbContext
		{
			var options = GetDbContextOptions<T>(loggerFactory, dbName);
			var ctx = ctxFactory(options);
			ctx.Database.EnsureCreated();

			return ctx;
		}

		private static DbContextOptions<T> GetDbContextOptions<T>(ILoggerFactory loggerFactory, string dbName)
			where T : DbContext
		{
			return new DbContextOptionsBuilder<T>()
					.UseSqlServer($"Server=(local);Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true")
					.UseLoggerFactory(loggerFactory)
					.Options;
		}
	}
}
