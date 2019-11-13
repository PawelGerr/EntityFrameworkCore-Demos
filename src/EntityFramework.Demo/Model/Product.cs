using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace EntityFramework.Demo.Model
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public Guid GroupId { get; set; }
		public virtual ProductGroup Group { get; set; }

      public ulong RowVersion { get; set; }

      public virtual List<ProductTranslation> Translations { get; set; }

		public override string ToString()
      {
         var sb = new StringBuilder();
         sb.Append("{ Name = ").Append(Name);

         if (Translations?.Any() == true)
         {
            sb.Append(", Translations = { ");

            for (var index = 0; index < Translations.Count; index++)
            {
               if (index != 0)
                  sb.Append(", ");

               sb.Append(Translations[index]);
            }

            sb.Append(" }");
         }

         sb.Append(" }");
			return sb.ToString();
		}
	}
}
