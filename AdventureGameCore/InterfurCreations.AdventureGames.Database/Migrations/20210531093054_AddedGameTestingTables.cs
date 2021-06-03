using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedGameTestingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameTesting_GameSave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(nullable: true),
                    StateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_GameSave", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_MiscData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    GameName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_MiscData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_OptionVisited",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionId = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    GameName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_OptionVisited", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_StateVisited",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateId = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    GameName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_StateVisited", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_EndState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndState = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    GameName = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    LatestGameSaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_EndState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTesting_EndState_GameTesting_GameSave_LatestGameSaveId",
                        column: x => x.LatestGameSaveId,
                        principalTable: "GameTesting_GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_Error",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorMessage = table.Column<string>(nullable: true),
                    GameName = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    LatestGameSaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_Error", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTesting_Error_GameTesting_GameSave_LatestGameSaveId",
                        column: x => x.LatestGameSaveId,
                        principalTable: "GameTesting_GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_GameSaveData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataName = table.Column<string>(nullable: true),
                    DataValue = table.Column<string>(nullable: true),
                    GameTestingGameSaveId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_GameSaveData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTesting_GameSaveData_GameTesting_GameSave_GameTestingGameSaveId",
                        column: x => x.GameTestingGameSaveId,
                        principalTable: "GameTesting_GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_Grammar",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    SpellingMessage = table.Column<string>(nullable: true),
                    LatestGameSaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_Grammar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTesting_Grammar_GameTesting_GameSave_LatestGameSaveId",
                        column: x => x.LatestGameSaveId,
                        principalTable: "GameTesting_GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTesting_Warning",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarningMessage = table.Column<string>(nullable: true),
                    TimesOccured = table.Column<int>(nullable: false),
                    GameName = table.Column<string>(nullable: true),
                    LatestGameSaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTesting_Warning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTesting_Warning_GameTesting_GameSave_LatestGameSaveId",
                        column: x => x.LatestGameSaveId,
                        principalTable: "GameTesting_GameSave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameTesting_EndState_LatestGameSaveId",
                table: "GameTesting_EndState",
                column: "LatestGameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTesting_Error_LatestGameSaveId",
                table: "GameTesting_Error",
                column: "LatestGameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTesting_GameSaveData_GameTestingGameSaveId",
                table: "GameTesting_GameSaveData",
                column: "GameTestingGameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTesting_Grammar_LatestGameSaveId",
                table: "GameTesting_Grammar",
                column: "LatestGameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTesting_Warning_LatestGameSaveId",
                table: "GameTesting_Warning",
                column: "LatestGameSaveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTesting_EndState");

            migrationBuilder.DropTable(
                name: "GameTesting_Error");

            migrationBuilder.DropTable(
                name: "GameTesting_GameSaveData");

            migrationBuilder.DropTable(
                name: "GameTesting_Grammar");

            migrationBuilder.DropTable(
                name: "GameTesting_MiscData");

            migrationBuilder.DropTable(
                name: "GameTesting_OptionVisited");

            migrationBuilder.DropTable(
                name: "GameTesting_StateVisited");

            migrationBuilder.DropTable(
                name: "GameTesting_Warning");

            migrationBuilder.DropTable(
                name: "GameTesting_GameSave");
        }
    }
}
