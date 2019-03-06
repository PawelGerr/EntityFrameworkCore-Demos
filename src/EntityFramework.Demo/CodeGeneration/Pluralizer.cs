using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;

namespace EntityFramework.Demo.CodeGeneration
{
   public class Pluralizer : IPluralizer
   {
      private readonly Inflector.Inflector _inflector;

      public Pluralizer([NotNull] Inflector.Inflector inflector)
      {
         _inflector = inflector ?? throw new ArgumentNullException(nameof(inflector));
      }

      /// <inheritdoc />
      public string Pluralize(string identifier)
      {
         if (identifier == "People")
            return identifier;

         return _inflector.Pluralize(identifier);
      }

      /// <inheritdoc />
      public string Singularize(string identifier)
      {
         return _inflector.Singularize(identifier);
      }
   }
}
