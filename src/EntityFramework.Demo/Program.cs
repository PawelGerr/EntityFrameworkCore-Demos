using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EntityFramework.Demo.Demos;
using EntityFramework.Demo.Model;
using EntityFramework.Demo.TphModel.CodeFirst;
using EntityFramework.Demo.TptModel.CodeFirst;
using EntityFramework.Demo.TptModel.DatabaseFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
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

			ExecuteDemoDbQueries(loggerFactory, logger);
			ExecuteTphQueries(loggerFactory, logger);
			ExecuteTptQueries(loggerFactory, logger);
			ExecuteTptDatabaseFirstQueries(loggerFactory, logger);

			// DebugScaffolding();
		}

		private static void ExecuteDemoDbQueries(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
		{
			using (var ctx = GetDemoContext(loggerFactory))
			{
				ctx.Database.EnsureCreated();

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
				ctx.Database.Migrate();

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
				ctx.Database.Migrate();

				if (!ctx.People.Any())
					ctx.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(Tpt_Queries));

				var demos = new Tpt_Queries(ctx, logger);
				demos.FetchCustomers();
				demos.FetchEmployees();
			}
		}

		private static void ExecuteTptDatabaseFirstQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetScaffoldedTptContext(loggerFactory))
			{
				var demos = new Tpt_DatabseFirst_Queries(ctx, logger);

				if (!ctx.People.Any())
					demos.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(Tpt_DatabseFirst_Queries));

				demos.FetchCustomers();
				demos.FetchEmployees();
			}
		}

		private static void DebugScaffolding()
		{
			var services = new ServiceCollection()
				.AddEntityFrameworkDesignTimeServices();

			var serviceProvider = services.BuildServiceProvider();

			var operationReporter = serviceProvider.GetRequiredService<IOperationReporter>();
			var currentAssembly = Assembly.GetExecutingAssembly();
			var projDir = Environment.CurrentDirectory;
			var designArgs = Array.Empty<string>();

			var dbOperations = new DatabaseOperations(operationReporter, currentAssembly, projDir, "EntityFramework.Demo", "C#", designArgs);
			var files = dbOperations.ScaffoldContext(
																 "Microsoft.EntityFrameworkCore.SqlServer",
																 "Server=(local);Database=TptDemo;Trusted_Connection=True;MultipleActiveResultSets=true",
																 Path.Combine(projDir, "./TptModel/DatabaseFirst"),
																 Path.Combine(projDir, "./TptModel/DatabaseFirst"),
																 "ScaffoldedTptDbContext",
																 Enumerable.Empty<string>(),
																 Enumerable.Empty<string>(),
																 false,
																 true,
																 false
																);

		}

		private static ILoggerFactory GetLoggerFactory()
		{
			var serilog = new LoggerConfiguration()
								.WriteTo.Console()
								.MinimumLevel.Information()
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

		public static ScaffoldedTptDbContext GetScaffoldedTptContext(ILoggerFactory loggerFactory)
		{
			return GetDbContext<ScaffoldedTptDbContext>(loggerFactory, "TptDemo", o => new ScaffoldedTptDbContext(o));
		}

		private static T GetDbContext<T>(ILoggerFactory loggerFactory, string dbName, Func<DbContextOptions<T>, T> ctxFactory)
			where T : DbContext
		{
			var options = GetDbContextOptions<T>(loggerFactory, dbName);
			return ctxFactory(options);
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
