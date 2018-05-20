using System;
using System.Linq;
using EntityFramework.Demo.Demos.Dtos;
using EntityFramework.Demo.TptModel.CodeFirst;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
	public class Tpt_Queries
	{
		private readonly TptDbContext _ctx;
		private readonly ILogger _logger;

		public Tpt_Queries(TptDbContext ctx, ILogger logger)
		{
			_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
