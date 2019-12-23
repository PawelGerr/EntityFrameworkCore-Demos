using System;

namespace EntityFramework.Demo.TphModel.CodeFirst
{
   public class PersonTph
   {
      public Guid Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }

#nullable disable
      public PersonTph()
      {
      }
#nullable enable
   }
}
