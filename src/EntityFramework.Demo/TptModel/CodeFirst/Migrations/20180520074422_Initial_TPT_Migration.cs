using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Demo.TptModel.CodeFirst.Migrations
{
	public partial class Initial_TPT_Migration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable("People", table => new
																			{
																				Id = table.Column<Guid>(nullable: false),
																				FirstName = table.Column<string>(nullable: true),
																				LastName = table.Column<string>(nullable: true)
																			},
													constraints: table => table.PrimaryKey("PK_People", x => x.Id));

			migrationBuilder.CreateTable("Customers",
													table => new
																{
																	Id = table.Column<Guid>(nullable: false),
																	DateOfBirth = table.Column<DateTime>(nullable: false)
																},
													constraints: table =>
																	{
																		table.PrimaryKey("PK_Customers", x => x.Id);
																		table.ForeignKey("FK_Customers_People_Id", x => x.Id, "People", "Id", onDelete: ReferentialAction.Cascade);
																	});

			migrationBuilder.CreateTable("Employees",
													table => new
																{
																	Id = table.Column<Guid>(nullable: false),
																	Turnover = table.Column<decimal>(nullable: false)
																},
													constraints: table =>
																	{
																		table.PrimaryKey("PK_Employees", x => x.Id);
																		table.ForeignKey("FK_Employees_People_Id", x => x.Id, "People", "Id", onDelete: ReferentialAction.Cascade);
																	});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable("Customers");
			migrationBuilder.DropTable("Employees");
			migrationBuilder.DropTable("People");
		}
	}
}
