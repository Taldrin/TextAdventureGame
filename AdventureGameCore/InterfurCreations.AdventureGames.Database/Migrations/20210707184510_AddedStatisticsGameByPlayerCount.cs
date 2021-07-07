using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedStatisticsGameByPlayerCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatisticsGamesByPlayerCount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(nullable: true),
                    TotalPlayed = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticsGamesByPlayerCount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticsGamesByPlayerCount");
        }
    }
}
