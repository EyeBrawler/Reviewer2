using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reviewer2.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaperFileModelTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "PaperFiles",
                newName: "StoragePath");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "Papers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoragePath",
                table: "PaperFiles",
                newName: "StoredFileName");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "Papers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
