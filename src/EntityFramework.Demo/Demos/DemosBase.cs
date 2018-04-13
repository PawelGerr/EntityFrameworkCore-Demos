using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Demo.Model;
using Microsoft.Extensions.Logging;

namespace EntityFramework.Demo.Demos
{
	public abstract class DemosBase
	{
		protected DemoDbContext Context { get; }
		protected ILogger<DemosBase> Logger { get; }

		public DemosBase(DemoDbContext ctx, ILogger<DemosBase> logger)
		{
			Context = ctx ?? throw new ArgumentNullException(nameof(ctx));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected void Print(string caption, IEnumerable<ProductGroup> groups)
		{
			Logger.LogInformation("== {caption} ==", caption);

			foreach (var group in groups)
			{
				Logger.LogInformation("ProductGroup: {@group}, Products: {products} ", group, group.Products);
			}
		}

		protected void Print(string caption, IEnumerable items)
		{
			Logger.LogInformation("== {caption} ==", caption);

			foreach (var item in items)
			{
				Logger.LogInformation("{@item}", @item);
			}
		}
	}
}
