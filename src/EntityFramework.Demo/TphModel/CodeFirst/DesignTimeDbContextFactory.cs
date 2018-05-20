using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;

namespace EntityFramework.Demo.TphModel.CodeFirst
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TphDbContext>
	{
		/// <inheritdoc />
		public TphDbContext CreateDbContext(string[] args)
		{
			return Program.GetTphContext(NullLoggerFactory.Instance);
		}
	}
}
