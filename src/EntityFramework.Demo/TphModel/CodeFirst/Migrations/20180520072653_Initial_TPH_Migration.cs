using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Demo.TphModel.CodeFirst.Migrations
{
	public partial class Initial_TPH_Migration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable("People",
													table => new
																{
																	Id = table.Column<Guid>(nullable: false),
																	FirstName = table.Column<string>(nullable: true),
																	LastName = table.Column<string>(nullable: true),
																	DateOfBirth = table.Column<DateTime>(nullable: true),
																	Turnover = table.Column<decimal>(nullable: true),
																	Discriminator = table.Column<string>(nullable: false)
																},
													constraints: table => table.PrimaryKey("PK_People", x => x.Id));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable("People");
		}
	}
}
