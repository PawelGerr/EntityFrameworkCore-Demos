using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFramework.Demo.SchemaChange
{
	public class SchemaChangeProductEntityConfiguration : IEntityTypeConfiguration<SchemaChangeProduct>
	{
		private readonly string? _schema;

		public SchemaChangeProductEntityConfiguration(string? schema)
		{
			_schema = schema;
		}

		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<SchemaChangeProduct> builder)
		{
			if (!String.IsNullOrWhiteSpace(_schema))
				builder.ToTable(nameof(SchemaChangeDbContext.Products), _schema);

			builder.HasKey(product => product.Id);
		}
	}
}
