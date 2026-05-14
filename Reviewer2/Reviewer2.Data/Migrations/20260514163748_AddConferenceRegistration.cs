using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reviewer2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConferenceRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conference_registrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    affiliation = table.Column<string>(type: "text", nullable: true),
                    dietary_restrictions = table.Column<string>(type: "text", nullable: true),
                    accessibility_needs = table.Column<string>(type: "text", nullable: true),
                    attending_banquet = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conference_registrations", x => x.id);
                    table.ForeignKey(
                        name: "fk_conference_registrations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_conference_registrations_user_id",
                table: "conference_registrations",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conference_registrations");
        }
    }
}
