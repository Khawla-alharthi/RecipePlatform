using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipePlatform.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "feedback",
                table: "Ratings",
                newName: "Feedback");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Feedback",
                table: "Ratings",
                newName: "feedback");
        }
    }
}
