using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedGameFramestack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerFrameStack",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnStateId = table.Column<string>(nullable: true),
                    SaveId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFrameStack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerFrameStack_PlayerGameSave_SaveId",
                        column: x => x.SaveId,
                        principalTable: "PlayerGameSave",
                        principalColumn: "SaveId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFrameStack_SaveId",
                table: "PlayerFrameStack",
                column: "SaveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerFrameStack");
        }
    }
}
