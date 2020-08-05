using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerGameSave",
                columns: table => new
                {
                    SaveId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GameName = table.Column<string>(nullable: true),
                    StateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameSave", x => x.SaveId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameSaveData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    PlayerGameSaveSaveId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameSaveData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGameSaveData_PlayerGameSave_PlayerGameSaveSaveId",
                        column: x => x.PlayerGameSaveSaveId,
                        principalTable: "PlayerGameSave",
                        principalColumn: "SaveId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PlayerFlag = table.Column<string>(nullable: true),
                    ActiveGameSaveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_Players_PlayerGameSave_ActiveGameSaveId",
                        column: x => x.ActiveGameSaveId,
                        principalTable: "PlayerGameSave",
                        principalColumn: "SaveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscordPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<string>(nullable: false),
                    ChatId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordPlayers", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_DiscordPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    PlayerGameSaveId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => new { x.PlayerGameSaveId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_GameSaves_PlayerGameSave_PlayerGameSaveId",
                        column: x => x.PlayerGameSaveId,
                        principalTable: "PlayerGameSave",
                        principalColumn: "SaveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSaves_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    ActionName = table.Column<string>(nullable: true),
                    GameName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerActions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelegramPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<string>(nullable: false),
                    ChatId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramPlayers", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_TelegramPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_PlayerId",
                table: "GameSaves",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerActions_PlayerId",
                table: "PlayerActions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameSaveData_PlayerGameSaveSaveId",
                table: "PlayerGameSaveData",
                column: "PlayerGameSaveSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ActiveGameSaveId",
                table: "Players",
                column: "ActiveGameSaveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordPlayers");

            migrationBuilder.DropTable(
                name: "GameSaves");

            migrationBuilder.DropTable(
                name: "PlayerActions");

            migrationBuilder.DropTable(
                name: "PlayerGameSaveData");

            migrationBuilder.DropTable(
                name: "TelegramPlayers");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerGameSave");
        }
    }
}
