using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseLibrary.Api.Migrations
{
    public partial class AddMoreAuthorsForSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "LastName", "MainCategory" },
                values: new object[,]
                {
                    { new Guid("4b822af2-4a03-40c4-bea7-62aade333b32"), new DateTimeOffset(new DateTime(1999, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Sadek", "Mohamed", "Rum" },
                    { new Guid("1505dc65-0594-4e1e-98fb-49fb19e09db2"), new DateTimeOffset(new DateTime(1993, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Salem", "Ahmed", "Ships" },
                    { new Guid("0f7ab3e2-dc5c-4512-93d0-cea929ca7508"), new DateTimeOffset(new DateTime(2000, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Mohamed", "Shaaban", "Rum" },
                    { new Guid("6508b058-31fd-43da-890f-14fe8ed5f290"), new DateTimeOffset(new DateTime(1970, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Reda", "Mabrook", "Rum" },
                    { new Guid("1279039b-665d-4cc3-b0d7-d3c643878f84"), new DateTimeOffset(new DateTime(1975, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Haitham", "Mohsen", "Ships" },
                    { new Guid("b2193e38-2226-4985-91a0-57a535a9f0e5"), new DateTimeOffset(new DateTime(1990, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Mohamed", "Amin", "Ships" },
                    { new Guid("66adbead-f3a5-4558-92b2-f88301b067e3"), new DateTimeOffset(new DateTime(1990, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Mosh", "Hamedani", "Ships" },
                    { new Guid("4b1111bb-a637-4414-aba5-872a944a9fa1"), new DateTimeOffset(new DateTime(1990, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Tim", "Corey", "Rum" },
                    { new Guid("17f93f58-ca1b-4064-aaba-2072185ec130"), new DateTimeOffset(new DateTime(1985, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Amr", "Gaber", "Rum" },
                    { new Guid("72cf36f7-8f10-4ab4-9a92-cf7c5c1b6e29"), new DateTimeOffset(new DateTime(1985, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Eslam", "Gaber", "Rum" },
                    { new Guid("153552a4-cb9c-4566-8040-8d44cb7428f8"), new DateTimeOffset(new DateTime(1985, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Ahmed", "Wael", "Rum" },
                    { new Guid("93fe5856-6806-4b27-b1bd-8b98599ca6bd"), new DateTimeOffset(new DateTime(1985, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Toty", "El-Tablawy", "Love" },
                    { new Guid("11e09402-ffd8-4ef8-853e-35d8950c70ae"), new DateTimeOffset(new DateTime(1985, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Bosy", "El-Tablawy", "Ships" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("0f7ab3e2-dc5c-4512-93d0-cea929ca7508"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("11e09402-ffd8-4ef8-853e-35d8950c70ae"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("1279039b-665d-4cc3-b0d7-d3c643878f84"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("1505dc65-0594-4e1e-98fb-49fb19e09db2"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("153552a4-cb9c-4566-8040-8d44cb7428f8"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("17f93f58-ca1b-4064-aaba-2072185ec130"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("4b1111bb-a637-4414-aba5-872a944a9fa1"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("4b822af2-4a03-40c4-bea7-62aade333b32"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("6508b058-31fd-43da-890f-14fe8ed5f290"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("66adbead-f3a5-4558-92b2-f88301b067e3"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("72cf36f7-8f10-4ab4-9a92-cf7c5c1b6e29"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("93fe5856-6806-4b27-b1bd-8b98599ca6bd"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("b2193e38-2226-4985-91a0-57a535a9f0e5"));
        }
    }
}
