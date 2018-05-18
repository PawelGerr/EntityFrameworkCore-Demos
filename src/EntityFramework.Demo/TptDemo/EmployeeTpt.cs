using System;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework.Demo.TphModel;

namespace EntityFramework.Demo.TptDemo
{
	public class EmployeeTpt
	{
		[ForeignKey(nameof(Person))]
		public Guid Id { get; set; }

		public PersonTpt Person { get; set; }

		public decimal Turnover { get; set; }
	}
}
