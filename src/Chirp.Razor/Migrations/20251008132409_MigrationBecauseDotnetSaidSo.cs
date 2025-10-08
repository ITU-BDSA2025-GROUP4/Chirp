using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class MigrationBecauseDotnetSaidSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Authors_Email_Name",
                table: "Authors",
                columns: new[] { "Email", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Authors_Email_Name",
                table: "Authors");
        }
    }
}
