using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackFinder.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommunityAdminUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_AdminId",
                table: "Communities");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_AdminId",
                table: "Communities",
                column: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_AdminId",
                table: "Communities");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_AdminId",
                table: "Communities",
                column: "AdminId",
                unique: true);
        }
    }
}
