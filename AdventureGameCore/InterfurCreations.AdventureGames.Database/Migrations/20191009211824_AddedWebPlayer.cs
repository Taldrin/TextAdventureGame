using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedWebPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<string>(nullable: false),
                    AccessKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPlayers", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_WebPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebPlayers");
        }
    }
}
