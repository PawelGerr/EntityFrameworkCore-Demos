using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.TptModel.DatabaseFirst
{
   public partial class ScaffoldedTptDbContext : DbContext
   {
#nullable disable
      public virtual DbSet<Customer> Customers { get; set; }
      public virtual DbSet<Employee> Employees { get; set; }
      public virtual DbSet<Person> People { get; set; }
#nullable enable

      public ScaffoldedTptDbContext(DbContextOptions<ScaffoldedTptDbContext> options)
         : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Customer>(entity =>
                                       {
                                          entity.Property(e => e.Id).ValueGeneratedNever();

                                          entity.HasOne(d => d.Person)
                                                .WithOne(p => p.Customer)
                                                .HasForeignKey<Customer>(d => d.Id);
                                       });

         modelBuilder.Entity<Employee>(entity =>
                                       {
                                          entity.Property(e => e.Id).ValueGeneratedNever();

                                          entity.HasOne(d => d.Person)
                                                .WithOne(p => p.Employee)
                                                .HasForeignKey<Employee>(d => d.Id);
                                       });

         modelBuilder.Entity<Person>(entity =>
                                     {
                                        entity.Property(e => e.Id).ValueGeneratedNever();
                                        entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                                        entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                                     });
      }
   }
}
