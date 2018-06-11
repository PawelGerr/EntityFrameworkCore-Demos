using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Demo.Model;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
	public class SelectMany_Issue : DemosBase
	{
		public SelectMany_Issue(DemoDbContext ctx, ILogger logger)
			: base(ctx, logger)
		{
		}

		public void SelectMany_Throws()
		{
			try
			{
				// throws System.ArgumentException: must be reducible node

				// the cause is the access to outer entity, i.e. "g.Name"
				// https://github.com/aspnet/EntityFrameworkCore/issues/11933
				var products = Context.ProductGroups
													.SelectMany(g => g.Products.Select(p => new { Group = g.Name, p.Id, p.Name }))
													.ToList();
			}
			catch (Exception ex)
			{
				Logger.LogError(0, ex, "SelectMany_Throws");
			}
		}
	}
}
