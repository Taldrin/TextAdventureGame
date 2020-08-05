using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class moreAccessTokenDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HoursForRefresh",
                table: "AccessToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                table: "AccessToken",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursForRefresh",
                table: "AccessToken");

            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "AccessToken");
        }
    }
}
