using System;

namespace EntityFramework.Demo.TphModel.DatabaseFirst
{
	public class CustomerTph : PersonTph
	{
		public DateTime DateOfBirth { get; set; }
	}
}
