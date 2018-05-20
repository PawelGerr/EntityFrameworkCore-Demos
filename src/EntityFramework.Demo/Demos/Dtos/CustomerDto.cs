using System;

namespace EntityFramework.Demo.Demos.Dtos
{
	public class CustomerDto
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}
