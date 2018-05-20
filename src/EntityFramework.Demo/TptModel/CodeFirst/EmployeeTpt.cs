using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Demo.TptModel.CodeFirst
{
	public class EmployeeTpt
	{
		[ForeignKey(nameof(Person))]
		public Guid Id { get; set; }

		public PersonTpt Person { get; set; }

		public decimal Turnover { get; set; }
	}
}
