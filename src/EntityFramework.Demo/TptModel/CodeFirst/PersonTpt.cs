using System;

namespace EntityFramework.Demo.TptModel.CodeFirst
{
   public class PersonTpt
   {
      public Guid Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }

#nullable disable
      public PersonTpt()
      {
      }
#nullable enable
   }
}
