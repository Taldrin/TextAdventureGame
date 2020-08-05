using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class PlayerMenuContextAndNamedSaves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerMenuContext",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerMenuContext2",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SaveName",
                table: "PlayerGameSave",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerMenuContext",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerMenuContext2",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SaveName",
                table: "PlayerGameSave");
        }
    }
}
