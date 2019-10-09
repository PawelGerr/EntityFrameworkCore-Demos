using System;
using System.Linq;
using EntityFramework.Demo.Demos.Dtos;
using EntityFramework.Demo.TphModel.CodeFirst;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class Tph_Queries
   {
      private readonly TphDbContext _ctx;
      private readonly ILogger _logger;

      public Tph_Queries(TphDbContext ctx, ILogger logger)
      {
         _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      }

      /**
       * Executed query:
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
