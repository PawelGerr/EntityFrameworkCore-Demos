using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Demo.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public static class DatabaseFacadeExtensions
   {
      public static IDbContextTransaction BeginTransaction(this DatabaseFacade database, string name)
      {
         database.OpenConnection();

         var connection = (SqlConnection)database.GetDbConnection();
         var transaction = connection.BeginTransaction(name);

         return database.UseTransaction(transaction);
      }
   }

   public class NamedTransactionsDemo : DemosBase
   {
      private readonly DemoDbContext _anotherContext;

      /// <inheritdoc />
      public NamedTransactionsDemo(DemoDbContext ctx, DemoDbContext anotherContext, ILogger logger)
         : base(ctx, logger)
      {
         _anotherContext = anotherContext ?? throw new ArgumentNullException(nameof(anotherContext));
      }

      public async Task StartNamedTransactionAsync()
      {
         try
         {
            await using var tx = Context.Database.BeginTransaction("Transaction of conn.");
            await using var anotherTx = _anotherContext.Database.BeginTransaction("Transaction of anotherConn.");

            var product = Context.Products.FirstOrDefault();
            product.Name += "changed";
            Context.SaveChanges();

            var group = _anotherContext.ProductGroups.FirstOrDefault();
            group.Name += "changed";
            _anotherContext.SaveChanges();

            product = _anotherContext.Products.FirstOrDefault();
            group = Context.ProductGroups.FirstOrDefault();
            product.Name += "changed again";
            group.Name += "changed again";

            var saveTask = Context.SaveChangesAsync();
            var anotherSaveTask = _anotherContext.SaveChangesAsync();

            await Task.WhenAll(saveTask, anotherSaveTask);
         }
         catch (InvalidOperationException ex) when ((ex.InnerException?.InnerException is SqlException sqlEx) && sqlEx.Number == 1205)
         {
            Logger.LogInformation(ex, "Deadlock captured");
         }
      }
   }
}
