using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.SchemaChange
{
   public class SchemaChangeDbContext : DbContext, IDbContextSchema
   {
      public string? Schema { get; }

#nullable disable
      public DbSet<SchemaChangeProduct> Products { get; set; }
#nullable enable

      public SchemaChangeDbContext(
         DbContextOptions<SchemaChangeDbContext> options,
         IDbContextSchema? schema = null)
         : base(options)
      {
         Schema = schema?.Schema;
      }

      /// <inheritdoc />
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         modelBuilder.ApplyConfiguration(new SchemaChangeProductEntityConfiguration(Schema));
      }
   }
}
