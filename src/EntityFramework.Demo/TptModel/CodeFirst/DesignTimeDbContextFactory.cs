using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;

namespace EntityFramework.Demo.TptModel.CodeFirst
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TptDbContext>
	{
		/// <inheritdoc />
		public TptDbContext CreateDbContext(string[] args)
		{
			return Program.GetTptContext(NullLoggerFactory.Instance);
		}
	}
}
