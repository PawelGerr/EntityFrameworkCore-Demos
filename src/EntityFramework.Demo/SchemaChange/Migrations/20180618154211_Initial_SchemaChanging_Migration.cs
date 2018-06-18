using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Demo.SchemaChange.Migrations
{
	public partial class Initial_SchemaChanging_Migration : Migration
	{
		private readonly IDbContextSchema _schema;

		public Initial_SchemaChanging_Migration(IDbContextSchema schema)
		{
			_schema = schema ?? throw new ArgumentNullException(nameof(schema));
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable("Products",
													table => new { Id = table.Column<Guid>() },
													constraints: table => table.PrimaryKey("PK_Products", x => x.Id),
													schema: _schema.Schema);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable("Products", _schema.Schema);
		}
	}
}
