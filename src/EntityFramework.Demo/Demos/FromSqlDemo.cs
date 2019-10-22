using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class FromSqlDemo : DemosBase
   {
      public FromSqlDemo(DemoDbContext ctx, ILogger logger)
         : base(ctx, logger)
      {
      }

      public void FromSql_Projection_ToList()
      {
         /**
          * Generated query:
          *
          *    SELECT * FROM Products
          */
         var productIds = Context.Products
                                 .FromSqlRaw("SELECT * FROM Products")
                                 .Select(p => p.Id)
                                 .ToList();
      }

      public void FromSql_Projection_FirstOrDefault()
      {
         /**
          * Generated query:
          *
          * SELECT TOP(1) [p].[Id]
          * FROM (
          *    SELECT * FROM Products
          * ) AS [p]
          *
          */
         var productId = Context.Products
                                .FromSqlRaw("SELECT * FROM Products")
                                .Select(p => p.Id)
                                .FirstOrDefault();
      }
   }
}
