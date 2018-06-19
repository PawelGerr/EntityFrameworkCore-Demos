using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EntityFramework.Demo.Demos;
using EntityFramework.Demo.Model;
using EntityFramework.Demo.SchemaChange;
using EntityFramework.Demo.TphModel.CodeFirst;
using EntityFramework.Demo.TphModel.DatabaseFirst;
using EntityFramework.Demo.TptModel.CodeFirst;
using EntityFramework.Demo.TptModel.DatabaseFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EntityFramework.Demo
{
	public class Program
	{
		public static async Task Main()
		{
			var loggerFactory = GetLoggerFactory();
			var logger = loggerFactory.CreateLogger<DemosBase>();

			ExecuteDemoDbQueries(loggerFactory, loggerFactory.CreateLogger<N_Plus_One_Queries>());
			ExecuteTphQueries(loggerFactory, logger);
			ExecuteTptQueries(loggerFactory, logger);
			ExecuteTphDatabaseFirstQueries(loggerFactory, logger);
			ExecuteTptDatabaseFirstQueries(loggerFactory, logger);
			ExecuteSchemaChangeQueries(loggerFactory, logger);
			await ExecuteTransactionScopeDemosAsync(loggerFactory, loggerFactory.CreateLogger<TransactionScope_Limitations_Demos>());
			ExecuteSelectManyIssues(loggerFactory, logger);

			// DebugScaffolding();
		}

		private static void ExecuteSelectManyIssues(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
		{
			using (var ctx = GetDemoContext(loggerFactory))
			{
				ctx.Database.EnsureCreated();

				if (!ctx.Products.Any())
					ctx.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(SelectMany_Issue));
				var demos = new SelectMany_Issue(ctx, logger);

				demos.SelectMany_Throws();
			}
		}

		private static void ExecuteDemoDbQueries(ILoggerFactory loggerFactory, ILogger<N_Plus_One_Queries> logger)
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

		private static void ExecuteTphDatabaseFirstQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			using (var ctx = GetScaffoldedTphContext(loggerFactory))
			{
				var demos = new Tph_DatabseFirst_Queries(ctx, logger);

				if (!ctx.People.Any())
					demos.SeedData();

				logger.LogInformation(" ==== {caption} ====", nameof(Tph_DatabseFirst_Queries));

				demos.FetchCustomers();
				demos.FetchEmployees();
			}
		}

		private static void ExecuteSchemaChangeQueries(ILoggerFactory loggerFactory, ILogger logger)
		{
			logger.LogInformation(" ==== {caption} ====", nameof(SchemaChange_Queries));

			using (var ctx = GetSchemaChangeDbContext(loggerFactory))
			{
				ctx.Database.Migrate();

				var demos = new SchemaChange_Queries(ctx, logger);
				// Executes: SELECT [p].[Id] FROM [Products] AS [p]
				demos.FetchProducts();
			}

			using (var ctx = GetSchemaChangeDbContext(loggerFactory, "demo"))
			{
				ctx.Database.Migrate();

				var demos = new SchemaChange_Queries(ctx, logger);
				// Executes: SELECT [p].[Id] FROM [demo].[Products] AS [p]
				demos.FetchProducts();
			}

			using (var ctx = GetSchemaChangeDbContextViaServiceProvider(loggerFactory, "demo2"))
			{
				ctx.Database.Migrate();

				var demos = new SchemaChange_Queries(ctx, logger);
				// Executes: SELECT [p].[Id] FROM [demo2].[Products] AS [p]
				demos.FetchProducts();
			}
		}

		private static async Task ExecuteTransactionScopeDemosAsync(ILoggerFactory loggerFactory, ILogger<TransactionScope_Limitations_Demos> logger)
		{
			using (var ctx = GetDemoContext(loggerFactory))
			using (var anotherCtx = GetDemoContext(loggerFactory))
			{
				ctx.Database.EnsureCreated();

				var demos = new TransactionScope_Limitations_Demos(ctx, anotherCtx, logger);

				try
				{
					// throws System.InvalidOperationException: An ambient transaction has been detected. The ambient transaction needs to be completed before beginning a transaction on this connection.
					demos.Try_BeginTransaction_within_TransactionScope();
				}
				catch (Exception ex)
				{
					logger.LogError(0, ex, "Exception in Try_BeginTransaction_within_TransactionScope()");
				}

				try
				{
					// throws System.PlatformNotSupportedException: This platform does not support distributed transactions.
					demos.Try_multiple_DatabaseConnections_within_TransactionScope();
				}
				catch (Exception ex)
				{
					logger.LogError(0, ex, "Exception in Try_multiple_DatabaseConnections_within_TransactionScope()");
				}

				// no errors
				await demos.Try_await_within_TransactionScope_with_TransactionScopeAsyncFlowOption();

				try
				{
					// System.InvalidOperationException: A TransactionScope must be disposed on the same thread that it was created.
					await demos.Try_await_within_TransactionScope_without_TransactionScopeAsyncFlowOption();
				}
				catch (Exception ex)
				{
					logger.LogError(0, ex, "Exception in Try_await_within_TransactionScope()");
				}

				try
				{
					// throws because of the previous call Try_await_within_TransactionScope_without_TransactionScopeAsyncFlowOption
					// System.InvalidOperationException: Connection currently has transaction enlisted.  Finish current transaction and retry.
					await demos.Try_await_within_TransactionScope_with_TransactionScopeAsyncFlowOption();
				}
				catch (Exception ex)
				{
					logger.LogError(0, ex, "Exception in Try_await_within_TransactionScope()");
				}
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

		public static ScaffoldedTphDbContext GetScaffoldedTphContext(ILoggerFactory loggerFactory)
		{
			return GetDbContext<ScaffoldedTphDbContext>(loggerFactory, "TphDemo", o => new ScaffoldedTphDbContext(o));
		}

		public static ScaffoldedTptDbContext GetScaffoldedTptContext(ILoggerFactory loggerFactory)
		{
			return GetDbContext<ScaffoldedTptDbContext>(loggerFactory, "TptDemo", o => new ScaffoldedTptDbContext(o));
		}

		public static SchemaChangeDbContext GetSchemaChangeDbContext(ILoggerFactory loggerFactory, string schema = null)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SchemaChangeDbContext>()
										.UseSqlServer("Server=(local);Database=SchemaChangeDemo;Trusted_Connection=True;MultipleActiveResultSets=true"
														 // optional
														 , b => b.MigrationsHistoryTable("__EFMigrationsHistory", schema)
														)
										.UseLoggerFactory(loggerFactory)
										.ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
										.ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>();

			return new SchemaChangeDbContext(optionsBuilder.Options, schema == null ? null : new DbContextSchema(schema));
		}

		public static SchemaChangeDbContext GetSchemaChangeDbContextViaServiceProvider(ILoggerFactory loggerFactory, string schema = null)
		{
			var services = new ServiceCollection()
				.AddDbContext<SchemaChangeDbContext>(builder => builder.UseSqlServer("Server=(local);Database=SchemaChangeDemo;Trusted_Connection=True;MultipleActiveResultSets=true"
																											 // optional
																											 , b => b.MigrationsHistoryTable("__EFMigrationsHistory", schema)
																											)
																						.ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
																						.ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>());

			if (schema != null)
				services.AddSingleton<IDbContextSchema>(new DbContextSchema(schema));

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider.GetRequiredService<SchemaChangeDbContext>();
		}

		private static T GetDbContext<T>(ILoggerFactory loggerFactory, string dbName, Func<DbContextOptions<T>, T> ctxFactory)
			where T : DbContext
		{
			var optionsBuilder = GetDbContextOptionsBuilder<T>(loggerFactory, dbName);
			return ctxFactory(optionsBuilder.Options);
		}

		private static DbContextOptionsBuilder<T> GetDbContextOptionsBuilder<T>(ILoggerFactory loggerFactory, string dbName)
			where T : DbContext
		{
			return new DbContextOptionsBuilder<T>()
					.UseSqlServer($"Server=(local);Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true")
					.UseLoggerFactory(loggerFactory);
		}
	}
}
