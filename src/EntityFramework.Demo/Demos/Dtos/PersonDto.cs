using System;

namespace EntityFramework.Demo.Demos.Dtos
{
   public class PersonDto
   {
      public Guid Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }

#nullable disable
      public PersonDto()
      {
      }
#nullable enable
   }
}
