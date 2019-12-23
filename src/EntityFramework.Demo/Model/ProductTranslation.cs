using System;

namespace EntityFramework.Demo.Model
{
   public class ProductTranslation
   {
      public Guid ProductId { get; set; }
      public string Locale { get; set; }
      public string Description { get; set; }

#nullable disable
      public ProductTranslation()
      {
      }
#nullable enable

      public override string ToString()
      {
         return $"[{Locale}] = \"{Description}\"";
      }
   }
}
