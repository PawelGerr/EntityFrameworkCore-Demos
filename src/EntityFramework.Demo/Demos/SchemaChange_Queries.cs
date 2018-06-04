using System;
using System.Linq;
using EntityFramework.Demo.SchemaChange;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
	public class SchemaChange_Queries
	{
		private readonly SchemaChangeDbContext _ctx;
		private readonly ILogger _logger;

		public SchemaChange_Queries(SchemaChangeDbContext ctx, ILogger logger)
		{
			_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void FetchProducts()
		{
			_logger.LogInformation("The database schema is {schema}", _ctx.Schema ?? "not set");
			var products = _ctx.Products.ToList();
		}
	}
}
