using Microsoft.EntityFrameworkCore.Migrations;

namespace BotDatabase.Migrations
{
    public partial class ActivePlayerGameSave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGameSaveData_PlayerGameSave_PlayerGameSaveSaveId",
                table: "PlayerGameSaveData");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGameSaveData_PlayerGameSave_PlayerGameSaveSaveId",
                table: "PlayerGameSaveData",
                column: "PlayerGameSaveSaveId",
                principalTable: "PlayerGameSave",
                principalColumn: "SaveId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerGameSaveData_PlayerGameSave_PlayerGameSaveSaveId",
                table: "PlayerGameSaveData");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerGameSaveData_PlayerGameSave_PlayerGameSaveSaveId",
                table: "PlayerGameSaveData",
                column: "PlayerGameSaveSaveId",
                principalTable: "PlayerGameSave",
                principalColumn: "SaveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
