using System;
using System.Threading.Tasks;
using EntityFramework.Demo.SchemaChange;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.IntegrationTests
{
	public class DemoRepository
	{
		public const int PK_VIOLATION = 2627;

		private readonly SchemaChangeDbContext _dbContext;

		public DemoRepository(SchemaChangeDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<bool> AddProductAsync(Guid id)
		{
			try
			{
				using (var tx = _dbContext.Database.BeginTransaction())
				{
					_dbContext.Products.Add(new SchemaChangeProduct { Id = id });
					await _dbContext.SaveChangesAsync();

					tx.Commit();
				}
			}
			catch (DbUpdateException ex) when(ex.InnerException is SqlException sqlEx && sqlEx.Number == PK_VIOLATION)
			{
				// add logging
				return false;
			}

			return true;
		}
	}
}
