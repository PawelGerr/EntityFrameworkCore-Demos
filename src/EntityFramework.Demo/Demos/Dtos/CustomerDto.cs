using System;

namespace EntityFramework.Demo.Demos.Dtos
{
	public class CustomerDto : PersonDto
	{
		public DateTime DateOfBirth { get; set; }
	}
}
