using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reviewer2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "session_id",
                table: "papers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_papers_session_id",
                table: "papers",
                column: "session_id");

            migrationBuilder.AddForeignKey(
                name: "fk_papers_session_session_id",
                table: "papers",
                column: "session_id",
                principalTable: "session",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_papers_session_session_id",
                table: "papers");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropIndex(
                name: "ix_papers_session_id",
                table: "papers");

            migrationBuilder.DropColumn(
                name: "session_id",
                table: "papers");
        }
    }
}
