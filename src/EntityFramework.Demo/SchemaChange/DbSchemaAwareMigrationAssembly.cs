using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

#pragma warning disable EF1001

namespace EntityFramework.Demo.SchemaChange
{
   public class DbSchemaAwareMigrationAssembly : MigrationsAssembly
   {
      private readonly DbContext _context;

      /// <inheritdoc />
      public DbSchemaAwareMigrationAssembly(
         ICurrentDbContext currentContext,
         IDbContextOptions options,
         IMigrationsIdGenerator idGenerator,
         IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
         : base(currentContext, options, idGenerator, logger)
      {
         _context = currentContext.Context;
      }

      /// <inheritdoc />
      public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
      {
         if (activeProvider == null)
            throw new ArgumentNullException(nameof(activeProvider));

         var hasCtorWithSchema = migrationClass.GetConstructor(new[] { typeof(IDbContextSchema) }) != null;

         if (hasCtorWithSchema && _context is IDbContextSchema schema)
         {
            var instance = (Migration)Activator.CreateInstance(migrationClass.AsType(), schema)!;
            instance.ActiveProvider = activeProvider;
            return instance;
         }

         return base.CreateMigration(migrationClass, activeProvider);
      }
   }
}
