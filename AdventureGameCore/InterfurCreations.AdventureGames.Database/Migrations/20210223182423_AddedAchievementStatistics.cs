using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedAchievementStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatisticsGameAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(nullable: true),
                    AchievementName = table.Column<string>(nullable: true),
                    TotalPlayed = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticsGameAchievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatisticsPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatisticsName = table.Column<string>(nullable: true),
                    StatisticsValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticsPositions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticsGameAchievements");

            migrationBuilder.DropTable(
                name: "StatisticsPositions");
        }
    }
}
