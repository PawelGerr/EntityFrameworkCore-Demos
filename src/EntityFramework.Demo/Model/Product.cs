using System;

namespace EntityFramework.Demo.Model
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public Guid GroupId { get; set; }
		public ProductGroup Group { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
