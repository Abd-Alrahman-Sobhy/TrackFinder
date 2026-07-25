using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificateImageUrl",
                table: "Certificates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateImageUrl",
                table: "Certificates");
        }
    }
}
