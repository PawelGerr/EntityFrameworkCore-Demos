using System;

namespace EntityFramework.Demo.SchemaChange
{
	public interface IDbContextSchema
	{
		string Schema { get; }
	}
}
