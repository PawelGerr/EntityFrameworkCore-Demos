using Microsoft.EntityFrameworkCore.Design;

namespace EntityFramework.Demo.CodeGeneration
{
	public class Pluralizer : IPluralizer
	{
		/// <inheritdoc />
		public string Pluralize(string identifier)
		{
			if (identifier == "People")
				return identifier;

			return Inflector.Inflector.Pluralize(identifier);
		}

		/// <inheritdoc />
		public string Singularize(string identifier)
		{
			return Inflector.Inflector.Singularize(identifier);
		}
	}
}
