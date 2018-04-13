using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.Model
{
	public class ProductGroup
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public ICollection<Product> Products { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
