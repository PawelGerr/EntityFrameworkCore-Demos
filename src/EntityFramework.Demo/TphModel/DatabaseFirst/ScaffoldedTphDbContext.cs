using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.TphModel.DatabaseFirst
{
	public class ScaffoldedTphDbContext : DbContext
	{
		public virtual DbSet<PersonTph> People { get; set; }
		public virtual DbSet<CustomerTph> Customers { get; set; }
		public virtual DbSet<EmployeeTph> Employees { get; set; }

		public ScaffoldedTphDbContext(DbContextOptions<ScaffoldedTphDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PersonTph>(entity => entity.Property(e => e.Id).ValueGeneratedNever());
			modelBuilder.Entity<PersonTph>()
							.HasDiscriminator(person => person.Discriminator)
							.HasValue<PersonTph>(nameof(PersonTph))
							.HasValue<CustomerTph>(nameof(CustomerTph))
							.HasValue<EmployeeTph>(nameof(EmployeeTph));
		}
	}
}
