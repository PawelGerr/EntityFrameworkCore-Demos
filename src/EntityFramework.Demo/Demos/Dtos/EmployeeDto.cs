using System;

namespace EntityFramework.Demo.Demos.Dtos
{
	public class EmployeeDto
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal Turnover { get; set; }
	}
}
