using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergbookRentals.Data.Migrations
{
    /// <inheritdoc />
    public partial class Setseededmemberrolestotitlecase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e648d4e-a530-4cd4-b8d7-8be891780f71",
                column: "Name",
                value: "Member");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e648d4e-a530-4cd4-b8d7-8be891780f71",
                column: "Name",
                value: "member");
        }
    }
}
