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
using Microsoft.EntityFrameworkCore.Diagnostics;
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

         LazyLoadingDemo(loggerFactory);
         NavigationPropertiesAlternativeQueries(loggerFactory);
         await NamedTransactionsDemo(loggerFactory);
         FromSqlDemo(loggerFactory);
         GlobalFiltersDemo(loggerFactory);
         MethodTranslatorDemo(loggerFactory);
         ExecuteDemoDbQueries(loggerFactory, loggerFactory.CreateLogger<N_Plus_One_Queries>());
         ExecuteTphQueries(loggerFactory, logger);
         ExecuteTptQueries(loggerFactory, logger);
         ExecuteTphDatabaseFirstQueries(loggerFactory, logger);
         ExecuteTptDatabaseFirstQueries(loggerFactory, logger);
         ExecuteSchemaChangeQueries(loggerFactory, logger);
         await ExecuteTransactionScopeDemosAsync(loggerFactory, loggerFactory.CreateLogger<TransactionScope_Limitations_Demos>());
         ExecuteBaseTypeMemberAccessLimitationDemo(loggerFactory, logger);
         await ExecuteGroupByIssuesDemoAsync(loggerFactory, logger);

         // DebugScaffolding();
      }

      private static void LazyLoadingDemo(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<LazyLoadingDemo>();

         using var ctx = GetDemoContext(loggerFactory, true);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(LazyLoadingDemo));

         var demo = new LazyLoadingDemo(ctx, logger);
         demo.BuildLookup_with_change_tracking();
         demo.BuildLookup_without_change_tracking();
      }

      private static void NavigationPropertiesAlternativeQueries(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<NavigationPropertiesAlternativeQueriesDemo>();

         using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(NavigationPropertiesAlternativeQueries));

         var demo = new NavigationPropertiesAlternativeQueriesDemo(ctx, logger);
         demo.UseNavigationalProperty();
         demo.WithoutNavigationalProperty();
         demo.WithoutNavigationalPropertyAndWithoutParentDbSet();
      }

      private static async Task NamedTransactionsDemo(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<FromSqlDemo>();

         using var ctx = GetDemoContext(loggerFactory);
         using var ctx2 = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(NamedTransactionsDemo));

         var demo = new NamedTransactionsDemo(ctx, ctx2, logger);
         await demo.StartNamedTransactionAsync();
      }

      private static void FromSqlDemo(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<FromSqlDemo>();

         using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(FromSqlDemo));

         var demo = new FromSqlDemo(ctx, logger);
         demo.FromSql_Projection_ToList();
         demo.FromSql_Projection_FirstOrDefault();
      }

      private static void GlobalFiltersDemo(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<GlobalFiltersDemo>();

         using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(GlobalFiltersDemo));

         var demo = new GlobalFiltersDemo(ctx, logger);
         demo.LoadTranslationsWithoutFilter();
         demo.LoadTranslationsWithLocaleFilter("en");
      }

      private static void MethodTranslatorDemo(ILoggerFactory loggerFactory)
      {
         var logger = loggerFactory.CreateLogger<MethodTranslatorDemo>();

         using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(MethodTranslatorDemo));

         var demo = new MethodTranslatorDemo(ctx, logger);
         demo.MethodCallWithOneColumnIsTranslated();
         demo.MethodCallWithNewExpressionThrows();
         demo.MethodCallWithArrayThrows();
      }

      private static async Task ExecuteGroupByIssuesDemoAsync(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
      {
         await using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(ExecuteGroupByIssuesDemoAsync));

         try
         {
            var demo = new GroupBy_Issues_Demos(ctx, loggerFactory.CreateLogger<GroupBy_Issues_Demos>());
            await demo.GroupBy_ToList_ToListAsync_throws();
         }
         catch (Exception e)
         {
            logger.LogError(e, "Error");
         }
      }

      private static void ExecuteBaseTypeMemberAccessLimitationDemo(ILoggerFactory loggerFactory, ILogger<DemosBase> logger)
      {
         using var ctx = GetDemoContext(loggerFactory);

         ctx.Database.EnsureCreated();

         if (!ctx.Products.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(ExecuteBaseTypeMemberAccessLimitationDemo));

         try
         {
            var demo = new BaseTypeMemberAccess_Limitation_Demos(ctx, loggerFactory.CreateLogger<BaseTypeMemberAccess_Limitation_Demos>());
            demo.LoadData();
         }
         catch (Exception e)
         {
            logger.LogError(e, "Error");
         }
      }

      private static void ExecuteDemoDbQueries(ILoggerFactory loggerFactory, ILogger<N_Plus_One_Queries> logger)
      {
         using var ctx = GetDemoContext(loggerFactory);

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

         demos.FetchGroups_Select_Filtered_Products_via_GroupJoin_Throws();
         demos.FetchGroups_Select_Filtered_Products_via_Lookup();
      }

      private static void ExecuteTphQueries(ILoggerFactory loggerFactory, ILogger logger)
      {
         using var ctx = GetTphContext(loggerFactory);

         ctx.Database.Migrate();

         if (!ctx.People.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(Tph_Queries));

         var demos = new Tph_Queries(ctx, logger);
         demos.FetchCustomers();
         demos.FetchEmployees();
      }

      private static void ExecuteTptQueries(ILoggerFactory loggerFactory, ILogger logger)
      {
         using var ctx = GetTptContext(loggerFactory);

         ctx.Database.Migrate();

         if (!ctx.People.Any())
            ctx.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(Tpt_Queries));

         var demos = new Tpt_Queries(ctx, logger);
         demos.FetchCustomers();
         demos.FetchEmployees();
      }

      private static void ExecuteTptDatabaseFirstQueries(ILoggerFactory loggerFactory, ILogger logger)
      {
         using var ctx = GetScaffoldedTptContext(loggerFactory);

         var demos = new Tpt_DatabseFirst_Queries(ctx, logger);

         if (!ctx.People.Any())
            demos.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(Tpt_DatabseFirst_Queries));

         demos.FetchCustomers();
         demos.FetchEmployees();
      }

      private static void ExecuteTphDatabaseFirstQueries(ILoggerFactory loggerFactory, ILogger logger)
      {
         using var ctx = GetScaffoldedTphContext(loggerFactory);

         var demos = new Tph_DatabseFirst_Queries(ctx, logger);

         if (!ctx.People.Any())
            demos.SeedData();

         logger.LogInformation(" ==== {caption} ====", nameof(Tph_DatabseFirst_Queries));

         demos.FetchCustomers();
         demos.FetchEmployees();
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
         await using var ctx = GetDemoContext(loggerFactory);
         await using var anotherCtx = GetDemoContext(loggerFactory);

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

      private static void DebugScaffolding()
      {
         var services = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices();

         var serviceProvider = services.BuildServiceProvider();

         var operationReporter = serviceProvider.GetRequiredService<IOperationReporter>();
         var currentAssembly = Assembly.GetExecutingAssembly();
         var projDir = Environment.CurrentDirectory;
         var designArgs = Array.Empty<string>();

         var dbOperations = new DatabaseOperations(operationReporter, currentAssembly, currentAssembly, projDir, "EntityFramework.Demo", "C#", designArgs);
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

      private static DemoDbContext GetDemoContext(ILoggerFactory loggerFactory, bool withLazyLoading = false)
      {
         return GetDbContext<DemoDbContext>(loggerFactory, "Demo", o => new DemoDbContext(o), withLazyLoading);
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
                                                                   .UseLoggerFactory(loggerFactory)
                                                                   .ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
                                                                   .ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>());

         if (schema != null)
            services.AddSingleton<IDbContextSchema>(new DbContextSchema(schema));

         var serviceProvider = services.BuildServiceProvider();

         return serviceProvider.GetRequiredService<SchemaChangeDbContext>();
      }

      private static T GetDbContext<T>(
         ILoggerFactory loggerFactory,
         string dbName,
         Func<DbContextOptions<T>, T> ctxFactory,
         bool withLazyLoading = false)
         where T : DbContext
      {
         var optionsBuilder = GetDbContextOptionsBuilder<T>(loggerFactory, dbName, withLazyLoading);
         return ctxFactory(optionsBuilder.Options);
      }

      private static DbContextOptionsBuilder<T> GetDbContextOptionsBuilder<T>(
         ILoggerFactory loggerFactory,
         string dbName,
         bool withLazyLoading = false)
         where T : DbContext
      {
         var builder = new DbContextOptionsBuilder<T>()
                       .UseSqlServer($"Server=(local);Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true")
                       .UseLoggerFactory(loggerFactory)
                       .EnableSensitiveDataLogging();

         if (withLazyLoading)
            builder.UseLazyLoadingProxies();

         if (typeof(T) == typeof(DemoDbContext))
         {
            var extension = builder.Options.FindExtension<DemoDbContextOptionsExtension>() ?? new DemoDbContextOptionsExtension();
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
         }

         return builder;
      }
   }
}
