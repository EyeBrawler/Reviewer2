using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reviewer2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AcceptWithRevisionsForPapers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "decision_made_at_utc",
                table: "papers",
                newName: "last_decision_at_utc");

            migrationBuilder.RenameColumn(
                name: "decision_comment",
                table: "papers",
                newName: "decision_or_revision_comment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_decision_at_utc",
                table: "papers",
                newName: "decision_made_at_utc");

            migrationBuilder.RenameColumn(
                name: "decision_or_revision_comment",
                table: "papers",
                newName: "decision_comment");
        }
    }
}
