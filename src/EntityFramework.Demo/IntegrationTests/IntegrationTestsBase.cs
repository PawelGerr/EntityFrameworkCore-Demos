using System;
using EntityFramework.Demo.SchemaChange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Demo.IntegrationTests
{
	public abstract class IntegrationTestsBase<T> : IDisposable
		where T : DbContext
	{
		private readonly string _schema;
		private readonly string _historyTableName;
		private readonly DbContextOptions<T> _options;

		protected T DbContext { get; }

		protected IntegrationTestsBase(string databaseName)
		{
			_schema = Guid.NewGuid().ToString("N");
			_historyTableName = "__EFMigrationsHistory";

			_options = CreateOptions(databaseName);
			DbContext = CreateContext();
			DbContext.Database.Migrate();
		}

		protected abstract T CreateContext(DbContextOptions<T> options, IDbContextSchema schema);

		protected T CreateContext()
		{
			return CreateContext(_options, new DbContextSchema(_schema));
		}

		private DbContextOptions<T> CreateOptions(string databaseName)
		{
			return new DbContextOptionsBuilder<T>()
					.UseSqlServer($"Server=(local);Database={databaseName};Trusted_Connection=True", builder => builder.MigrationsHistoryTable(_historyTableName, _schema))
					.ReplaceService<IMigrationsAssembly, DbSchemaAwareMigrationAssembly>()
					.ReplaceService<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>()
					.Options;
		}

		public void Dispose()
		{
			DbContext.GetService<IMigrator>().Migrate("0");
			DbContext.Database.ExecuteSqlRaw($"DROP TABLE [{_schema}].[{_historyTableName}]");
			DbContext.Database.ExecuteSqlRaw($"DROP SCHEMA [{_schema}]");

			DbContext?.Dispose();
		}
	}
}
