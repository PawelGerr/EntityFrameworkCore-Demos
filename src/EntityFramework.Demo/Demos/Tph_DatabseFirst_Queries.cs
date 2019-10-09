using System;
using System.Linq;
using EntityFramework.Demo.Demos.Dtos;
using EntityFramework.Demo.TphModel.DatabaseFirst;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class Tph_DatabseFirst_Queries
   {
      private readonly ScaffoldedTphDbContext _ctx;
      private readonly ILogger _logger;

      public Tph_DatabseFirst_Queries(ScaffoldedTphDbContext ctx, ILogger logger)
      {
         _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      }

      public void SeedData()
      {
         DeleteAll();

         _ctx.Customers.Add(new CustomerTph
                            {
                               Id = Guid.NewGuid(),
                               FirstName = "John",
                               LastName = "Foo",
                               DateOfBirth = new DateTime(1980, 1, 1)
                            });

         _ctx.Employees.Add(new EmployeeTph
                            {
                               Id = Guid.NewGuid(),
                               FirstName = "Max",
                               LastName = "Bar",
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

      /**
       * Execute query:
       *
       * SELECT [p].[Id], [p].[FirstName], [p].[LastName], [p].[DateOfBirth]
       * FROM [People] AS [p]
       * WHERE [p].[Discriminator] = N'CustomerTph'
       */
      public void FetchCustomers()
      {
         var customers = _ctx.Customers
                             .Select(c => new CustomerDto
                                          {
                                             Id = c.Id,
                                             FirstName = c.FirstName,
                                             LastName = c.LastName,
                                             DateOfBirth = c.DateOfBirth
                                          })
                             .ToList();

         _logger.LogInformation("[TPH] Customers: {@customers}", customers);
      }

      /**
       * Executed query:
       *
       * SELECT [p].[Id], [p].[FirstName], [p].[LastName], [p].[Turnover]
       * FROM [People] AS [p]
       * WHERE [p].[Discriminator] = N'EmployeeTph'
       */
      public void FetchEmployees()
      {
         var employees = _ctx.Employees
                             .Select(c => new EmployeeDto
                                          {
                                             Id = c.Id,
                                             FirstName = c.FirstName,
                                             LastName = c.LastName,
                                             Turnover = c.Turnover
                                          })
                             .ToList();

         _logger.LogInformation("[TPH] Employees: {@employees}", employees);
      }
   }
}
