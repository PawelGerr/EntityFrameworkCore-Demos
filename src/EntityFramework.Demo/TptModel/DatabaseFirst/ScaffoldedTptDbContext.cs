using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.Demo.TptModel.DatabaseFirst
{
	public partial class ScaffoldedTptDbContext : DbContext
	{
		public virtual DbSet<Customer> Customers { get; set; }
		public virtual DbSet<Employee> Employees { get; set; }
		public virtual DbSet<Person> People { get; set; }

		public ScaffoldedTptDbContext(DbContextOptions<ScaffoldedTptDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Customer>(entity =>
													{
														entity.Property(e => e.Id).ValueGeneratedNever();

														entity.HasOne(d => d.IdNavigation)
																.WithOne(p => p.Customer)
																.HasForeignKey<Customer>(d => d.Id);
													});

			modelBuilder.Entity<Employee>(entity =>
													{
														entity.Property(e => e.Id).ValueGeneratedNever();

														entity.HasOne(d => d.IdNavigation)
																.WithOne(p => p.Employee)
																.HasForeignKey<Employee>(d => d.Id);
													});

			modelBuilder.Entity<Person>(entity => entity.Property(e => e.Id).ValueGeneratedNever());
		}
	}
}
