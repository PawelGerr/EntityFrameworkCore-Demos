using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.Demo.Model
{
   public class DemoDbContext : DbContext
   {
#nullable disable
      public DbSet<Product> Products { get; set; }
      public DbSet<ProductGroup> ProductGroups { get; set; }
#nullable enable

      private string? _localeFilter;

      public DemoDbContext(DbContextOptions<DemoDbContext> options)
         : base(options)
      {
      }

      /// <inheritdoc />
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Product>(b =>
                                      {
                                         b.Property(p => p.Name).IsRequired().HasMaxLength(100);

                                         b.Property(p => p.RowVersion)
                                          .HasColumnType("RowVersion")
                                          .IsRowVersion()
                                          .HasConversion(new NumberToBytesConverter<ulong>());
                                      });

         modelBuilder.Entity<ProductTranslation>(b =>
                                                 {
                                                    b.Property(t => t.Locale).IsRequired().HasMaxLength(10);
                                                    b.Property(t => t.Description).IsRequired().HasMaxLength(200);
                                                    b.HasKey(t => new { t.ProductId, t.Locale });
                                                    b.HasQueryFilter(t => _localeFilter == null || t.Locale == _localeFilter);
                                                 });

         modelBuilder.Entity<ProductGroup>(b =>
                                           {
                                              b.Property(p => p.Name).IsRequired().HasMaxLength(200);
                                              b.Property(p => p.RowVersion)
                                               .HasColumnType("RowVersion")
                                               .IsRowVersion()
                                               .HasConversion(new RowVersionValueConverter());
                                           });
      }

      public IDisposable SetTranslationFilter(string locale)
      {
         if (locale == null)
            throw new ArgumentNullException(nameof(locale));
         if (_localeFilter != null)
            throw new InvalidOperationException($"Changing a filter is not allowed. Current filter: {_localeFilter}. Provided filter: {locale}");

         _localeFilter = locale;

         return new FilterReset(this);
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
            var id = Guid.NewGuid();
            var name = $"Product {i}";
            var product = new Product
                          {
                             Id = id,
                             Name = name,
                             Group = productGroups[i % productGroups.Count],
                             Translations = new List<ProductTranslation>
                                            {
                                               new ProductTranslation
                                               {
                                                  ProductId = id,
                                                  Locale = "en",
                                                  Description = $"Description of the product '{name}'."
                                               },
                                               new ProductTranslation
                                               {
                                                  ProductId = id,
                                                  Locale = "de",
                                                  Description = $"Produktbeschreibung von '{name}'."
                                               }
                                            }
                          };

            Products.Add(product);
         }
      }

      private List<ProductGroup> SeedProductGroups(int numberOfProducts)
      {
         for (int i = 0; i < numberOfProducts; i++)
         {
            var productGroup = new ProductGroup
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

      private readonly struct FilterReset : IDisposable
      {
         private readonly DemoDbContext _ctx;

         public FilterReset(DemoDbContext ctx)
         {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
         }

         public void Dispose()
         {
            _ctx._localeFilter = null;
         }
      }
   }
}
