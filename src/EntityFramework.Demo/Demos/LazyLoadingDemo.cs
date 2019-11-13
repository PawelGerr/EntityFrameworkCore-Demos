using System;
using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
   public class LazyLoadingDemo : DemosBase
   {
      public LazyLoadingDemo(DemoDbContext ctx, ILogger logger)
         : base(ctx, logger)
      {
      }

      public void BuildLookup_with_change_tracking()
      {
         /*
          * Executes 6 queries:
          *    1 for loading products
          *    5 for
          */

         var lookup = Context.Products
                             .ToLookup(p => p.Group.Name);

         var productCount = lookup.Sum(g => g.Count());
         var groupCount = lookup.Count();
      }

      public void BuildLookup_without_change_tracking()
      {
         /*
          * Exception is thrown by default:
          *
          * System.InvalidOperationException:
          *    Error generated for warning 'Microsoft.EntityFrameworkCore.Infrastructure.DetachedLazyLoadingWarning:
          *    An attempt was made to lazy-load navigation property 'Group' on detached entity of type 'ProductProxy'.
          *    Lazy-loading is not supported for detached entities or entities that are loaded with 'AsNoTracking()'.'.
          *    This exception can be suppressed or logged by passing event ID 'CoreEventId.DetachedLazyLoadingWarning' to
          *    the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.
          */
         try
         {
            var lookup = Context.Products
                                .AsNoTracking()
                                .ToLookup(p => p.Group.Name);
         }
         catch (InvalidOperationException ex)
         {
            Logger.LogError(ex, "Using navigational loading with lazy loading and detached entities.");
         }
      }
   }
}
