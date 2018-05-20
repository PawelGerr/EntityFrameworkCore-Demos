using System;
using System.Linq;
using EntityFramework.Demo.Demos.Dtos;
using EntityFramework.Demo.TptModel.CodeFirst;
using EntityFramework.Demo.TptModel.DatabaseFirst;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
	public class Tpt_DatabseFirst_Queries
	{
		private readonly ScaffoldedTptDbContext _ctx;
		private readonly ILogger _logger;

		public Tpt_DatabseFirst_Queries(ScaffoldedTptDbContext ctx, ILogger logger)
		{
			_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void SeedData()
		{
			DeleteAll();

			_ctx.Customers.Add(new Customer
									{
										Person = new Person
													{
														Id = Guid.NewGuid(),
														FirstName = "John",
														LastName = "Foo"
													},
										DateOfBirth = new DateTime(1980, 1, 1)
									});

			_ctx.Employees.Add(new Employee
									{
										Person = new Person
													{
														Id = Guid.NewGuid(),
														FirstName = "Max",
														LastName = "Bar"
													},
										Turnover = 1000
									});

			_ctx.SaveChanges();
		}

		private void DeleteAll()
		{
			_ctx.Customers.RemoveRange(_ctx.Customers);
			_ctx.Employees.RemoveRange(_ctx.Employees);
			_ctx.People.RemoveRange(_ctx.People);

			_ctx.SaveChanges();
		}

		public void FetchCustomers()
		{
			var customers = _ctx.Customers
										.Select(c => new CustomerDto
														{
															Id = c.Id,
															FirstName = c.Person.FirstName,
															LastName = c.Person.LastName,
															DateOfBirth = c.DateOfBirth
														})
										.ToList();

			_logger.LogInformation("[TPT] Customers: {@customers}", customers);
		}

		public void FetchEmployees()
		{
			var employees = _ctx.Employees
										.Select(c => new EmployeeDto
														{
															Id = c.Id,
															FirstName = c.Person.FirstName,
															LastName = c.Person.LastName,
															Turnover = c.Turnover
														})
										.ToList();

			_logger.LogInformation("[TPT] Employees: {@employees}", employees);
		}
	}
}
