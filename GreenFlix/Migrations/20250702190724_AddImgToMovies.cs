 using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenFlix.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddImgToMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "Movies");
        }
    }
}
