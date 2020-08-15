using Microsoft.EntityFrameworkCore.Migrations;

namespace InterfurCreations.AdventureGames.Database.Migrations
{
    public partial class AddedRequiredToFrameStackForAutoDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFrameStack_PlayerGameSave_SaveId",
                table: "PlayerFrameStack");

            migrationBuilder.AlterColumn<int>(
                name: "SaveId",
                table: "PlayerFrameStack",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFrameStack_PlayerGameSave_SaveId",
                table: "PlayerFrameStack",
                column: "SaveId",
                principalTable: "PlayerGameSave",
                principalColumn: "SaveId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFrameStack_PlayerGameSave_SaveId",
                table: "PlayerFrameStack");

            migrationBuilder.AlterColumn<int>(
                name: "SaveId",
                table: "PlayerFrameStack",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFrameStack_PlayerGameSave_SaveId",
                table: "PlayerFrameStack",
                column: "SaveId",
                principalTable: "PlayerGameSave",
                principalColumn: "SaveId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
