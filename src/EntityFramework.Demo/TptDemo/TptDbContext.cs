using System;
using EntityFramework.Demo.TphModel;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Demo.TptDemo
{
	public class TptDbContext : DbContext
	{
		public DbSet<PersonTpt> People { get; set; }
		public DbSet<CustomerTpt> Customers { get; set; }
		public DbSet<EmployeeTpt> Employees { get; set; }

		public TptDbContext(DbContextOptions<TptDbContext> options)
			: base(options)
		{
		}

		public void SeedData()
		{
			DeleteAll();

			Customers.Add(new CustomerTpt()
								{
									Person = new PersonTpt()
												{
													Id = Guid.NewGuid(),
													FirstName = "John",
													LastName = "Foo"
												},
									DateOfBirth = new DateTime(1980, 1, 1)
								});

			Employees.Add(new EmployeeTpt()
								{
									Person = new PersonTpt()
												{
													Id = Guid.NewGuid(),
													FirstName = "Max",
													LastName = "Bar"
												},
									Turnover = 1000
								});

			SaveChanges();
		}

		private void DeleteAll()
		{
			Customers.RemoveRange(Customers);
			Employees.RemoveRange(Employees);
			People.RemoveRange(People);

			SaveChanges();
		}
	}
}
