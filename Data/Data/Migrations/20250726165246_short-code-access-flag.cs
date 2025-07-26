using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class shortcodeaccessflag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClickedByShortUrl",
                table: "BookmarkClicks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClickedByShortUrl",
                table: "BookmarkClicks");
        }
    }
}
