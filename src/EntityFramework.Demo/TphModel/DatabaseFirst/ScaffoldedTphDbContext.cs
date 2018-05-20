using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.Demo.TphModel.DatabaseFirst
{
	public partial class ScaffoldedTphDbContext : DbContext
	{
		public virtual DbSet<Person> People { get; set; }

		public ScaffoldedTphDbContext(DbContextOptions<ScaffoldedTphDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Person>(entity =>
												{
													entity.Property(e => e.Id).ValueGeneratedNever();
													entity.Property(e => e.Discriminator).IsRequired();
												});
		}
	}
}
