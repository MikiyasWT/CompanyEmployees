using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class Databaseconfirmateionupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16b07077-9e9c-4849-955a-d5895a045be5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "739d114e-64af-4581-8ada-1da466a1d669");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8a979d69-127a-4c0b-8539-bdab0c71293c", null, "Administrator", "ADMINISTRATOR" },
                    { "a4b00d9c-f1df-4f79-8f8c-4378b8fd0c51", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a979d69-127a-4c0b-8539-bdab0c71293c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4b00d9c-f1df-4f79-8f8c-4378b8fd0c51");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "16b07077-9e9c-4849-955a-d5895a045be5", null, "Manager", "MANAGER" },
                    { "739d114e-64af-4581-8ada-1da466a1d669", null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
