using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Demo.TptModel.CodeFirst
{
	public class CustomerTpt
	{
		[ForeignKey(nameof(Person))]
		public Guid Id { get; set; }

		public PersonTpt Person { get; set; }

		public DateTime DateOfBirth { get; set; }
	}
}
