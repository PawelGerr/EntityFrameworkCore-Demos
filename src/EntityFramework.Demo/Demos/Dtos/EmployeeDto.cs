using System;

namespace EntityFramework.Demo.Demos.Dtos
{
	public class EmployeeDto : PersonDto
	{
		public decimal Turnover { get; set; }
	}
}
