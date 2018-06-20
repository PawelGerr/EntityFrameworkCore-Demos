using System;
using System.Threading.Tasks;
using EntityFramework.Demo.SchemaChange;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFramework.Demo.IntegrationTests
{
	public class DemoRepositoryTests : IntegrationTestsBase<SchemaChangeDbContext>
	{
		private readonly DemoRepository _repository;

		public DemoRepositoryTests()
			: base("SchemaChangeDemo")
		{
			_repository = new DemoRepository(DbContext);
		}

		protected override SchemaChangeDbContext CreateContext(DbContextOptions<SchemaChangeDbContext> options, IDbContextSchema schema)
		{
			return new SchemaChangeDbContext(options, schema);
		}

		[Fact]
		public async Task Should_add_new_product()
		{
			var productId = new Guid("DBD9439E-6FFD-4719-93C7-3F7FA64D2220");

			await _repository.AddProductAsync(productId);

			var insertedProduct = await DbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
			insertedProduct.Should().NotBeNull();
			insertedProduct.Id.Should().Be(productId);
		}

		[Fact]
		public async Task Should_return_false_on_conflict()
		{
			var productId = new Guid("DBD9439E-6FFD-4719-93C7-3F7FA64D2220");

			var newCtx = CreateContext();
			newCtx.Products.Add(new SchemaChangeProduct { Id =  productId});
			await newCtx.SaveChangesAsync();

			var result = await _repository.AddProductAsync(productId);

			result.Should().BeFalse();
		}
	}
}
