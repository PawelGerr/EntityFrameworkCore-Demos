using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class TransactionScope_Limitations_Demos : DemosBase
   {
      private readonly DemoDbContext _anotherCtx;

      /// <inheritdoc />
      public TransactionScope_Limitations_Demos(DemoDbContext ctx, DemoDbContext anotherCtx, ILogger<TransactionScope_Limitations_Demos> logger)
         : base(ctx, logger)
      {
         _anotherCtx = anotherCtx;
      }

      public void Try_BeginTransaction_within_TransactionScope()
      {
         using var scope = new TransactionScope();
         using var tx = Context.Database.BeginTransaction();

         Print("Try_BeginTransaction_within_TransactionScope", Context.ProductGroups);
      }

      public void Try_multiple_DatabaseConnections_within_TransactionScope()
      {
         using var scope = new TransactionScope();

         Print("Try_BeginTransaction_within_TransactionScope with Context", Context.ProductGroups);
         Print("Try_BeginTransaction_within_TransactionScope with AnotherCtx", _anotherCtx.ProductGroups);
      }

      public async Task Try_await_within_TransactionScope_without_TransactionScopeAsyncFlowOption()
      {
         using var scope = new TransactionScope();

         var groups = await Context.ProductGroups.ToListAsync();
         Print("Try_await_within_TransactionScope", groups);
      }

      public async Task Try_await_within_TransactionScope_with_TransactionScopeAsyncFlowOption()
      {
         using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

         var groups = await Context.ProductGroups.ToListAsync();
         Print("Try_await_within_TransactionScope", groups);
      }
   }
}
