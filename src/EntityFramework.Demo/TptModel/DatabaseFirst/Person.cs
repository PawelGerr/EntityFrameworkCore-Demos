using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.TptModel.DatabaseFirst
{
   public partial class Person
   {
      public Guid Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }

      public Customer Customer { get; set; }
      public Employee Employee { get; set; }

#nullable disable
      public Person()
      {
      }
#nullable enable
   }
}
