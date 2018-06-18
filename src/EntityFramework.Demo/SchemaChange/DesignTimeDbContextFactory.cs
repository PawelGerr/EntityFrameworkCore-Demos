using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;

namespace EntityFramework.Demo.SchemaChange
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SchemaChangeDbContext>
	{
		/// <inheritdoc />
		public SchemaChangeDbContext CreateDbContext(string[] args)
		{
			return Program.GetSchemaChangeDbContext(NullLoggerFactory.Instance);
		}
	}
}
