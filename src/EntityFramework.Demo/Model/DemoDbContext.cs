using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.Demo.Model
{
	public class DemoDbContext : DbContext
	{
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductGroup> ProductGroups { get; set; }

		public DemoDbContext(DbContextOptions<DemoDbContext> options)
			: base(options)
		{
		}

      /// <inheritdoc />
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Product>()
                     .Property(p => p.RowVersion)
                     .HasColumnType("RowVersion")
                     .IsRowVersion()
                     .HasConversion(new NumberToBytesConverter<ulong>());

         modelBuilder.Entity<ProductGroup>()
                     .Property(p => p.RowVersion)
                     .HasColumnType("RowVersion")
                     .IsRowVersion()
                     .HasConversion(new RowVersionValueConverter());
      }

		public void SeedData()
		{
			DeleteAll();

			var productGroups = SeedProductGroups(5);
			SeedProducts(100, productGroups);

			SaveChanges();
		}

		private void SeedProducts(int numberOfProducts, List<ProductGroup> productGroups)
		{
			for (int i = 0; i < numberOfProducts; i++)
			{
				var product = new Product()
				{
					Id = Guid.NewGuid(),
					Name = $"Product {i}",
					Group = productGroups[i % productGroups.Count]
				};

				Products.Add(product);
			}
		}

		private List<ProductGroup> SeedProductGroups(int numberOfProducts)
		{
			for (int i = 0; i < numberOfProducts; i++)
			{
				var productGroup = new ProductGroup()
				{
					Id = Guid.NewGuid(),
					Name = $"Product Group {i}"
				};

				ProductGroups.Add(productGroup);
			}

			SaveChanges();

			return ProductGroups.ToList();
		}

		private void DeleteAll()
		{
			Products.RemoveRange(Products);
			ProductGroups.RemoveRange(ProductGroups);

			SaveChanges();
		}
	}
}
