using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPerformanceTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfilePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Students",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 14, 13, 27, 49, 752, DateTimeKind.Utc).AddTicks(4490));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 14, 12, 16, 59, 723, DateTimeKind.Utc).AddTicks(9952));
        }
    }
}
