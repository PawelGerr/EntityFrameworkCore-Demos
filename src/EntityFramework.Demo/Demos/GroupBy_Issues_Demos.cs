using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class GroupBy_Issues_Demos : DemosBase
   {
      /// <inheritdoc />
      public GroupBy_Issues_Demos(DemoDbContext ctx, ILogger<GroupBy_Issues_Demos> logger)
         : base(ctx, logger)
      {
      }

      public async Task GroupBy_ToList_ToListAsync_throws()
      {
         var query = Context.ProductGroups
                            .Select(g => new
                                         {
                                            g.Id,
                                            Products = g.Products.ToList()
                                         })
                            .GroupBy(g => g.Id);

         query.ToList();
         Logger.LogInformation("Loading groups with products using ToList suceeded.");

         try
         {
            await query.ToListAsync();
            Logger.LogInformation("Loading groups with products using ToListAsync suceeded.");
         }
         catch (Exception ex)
         {
            Logger.LogError(ex, "Loading groups with products using ToListAsync failed.");
         }
      }
   }
}
