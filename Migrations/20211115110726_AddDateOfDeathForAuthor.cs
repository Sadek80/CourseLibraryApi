using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseLibrary.Api.Migrations
{
    public partial class AddDateOfDeathForAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfDeath",
                table: "Authors",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("93fe5856-6806-4b27-b1bd-8b98599ca6bd"),
                column: "FirstName",
                value: "Toty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "Authors");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("93fe5856-6806-4b27-b1bd-8b98599ca6bd"),
                column: "FirstName",
                value: "Tote");
        }
    }
}
