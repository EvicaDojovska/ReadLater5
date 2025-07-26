using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class extendentitiesuserbasis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookmarks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookmarks");
        }
    }
}
