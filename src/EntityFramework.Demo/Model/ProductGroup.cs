using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.Model
{
   public class ProductGroup
   {
      public Guid Id { get; set; }
      public string Name { get; set; }

      public ulong RowVersion { get; set; }

      public virtual ICollection<Product> Products { get; set; }

#nullable disable
      public ProductGroup()
      {
      }
#nullable enable

      public override string ToString()
      {
         return Name;
      }
   }
}
