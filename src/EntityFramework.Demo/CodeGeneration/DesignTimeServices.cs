using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Demo.CodeGeneration
{
	public class DesignTimeServices : IDesignTimeServices
	{
		/// <inheritdoc />
		public void ConfigureDesignTimeServices(IServiceCollection services)
		{
			services.AddSingleton<IPluralizer, Pluralizer>();
		}
	}
}
