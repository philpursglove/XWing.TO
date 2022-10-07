using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XWingTO.Data.Migrations
{
    public partial class CreationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentPlayers_Player1Id",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentPlayers_Player2Id",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentRound_TournamentRoundId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentRound_Tournaments_TournamentId",
                table: "TournamentRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentRound",
                table: "TournamentRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Game",
                table: "Game");

            migrationBuilder.RenameTable(
                name: "TournamentRound",
                newName: "TournamentRounds");

            migrationBuilder.RenameTable(
                name: "Game",
                newName: "Games");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentRound_TournamentId",
                table: "TournamentRounds",
                newName: "IX_TournamentRounds_TournamentId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_TournamentRoundId",
                table: "Games",
                newName: "IX_Games_TournamentRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_Player2Id",
                table: "Games",
                newName: "IX_Games_Player2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Game_Player1Id",
                table: "Games",
                newName: "IX_Games_Player1Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Tournaments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.DropPrimaryKey("PK_AspNetUserTokens", "AspNetUserTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey("PK_AspNetUserTokens", "AspNetUserTokens",
	            new string[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.DropPrimaryKey("PK_AspNetUserLogins", "AspNetUserLogins");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey("PK_AspNetUserLogins", "AspNetUserLogins",
	            new[] {"LoginProvider", "ProviderKey"});

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentRounds",
                table: "TournamentRounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Games",
                table: "Games",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_TournamentRounds_TournamentRoundId",
                table: "Games",
                column: "TournamentRoundId",
                principalTable: "TournamentRounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentRounds_Tournaments_TournamentId",
                table: "TournamentRounds",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_TournamentPlayers_Player1Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_TournamentPlayers_Player2Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_TournamentRounds_TournamentRoundId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentRounds_Tournaments_TournamentId",
                table: "TournamentRounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentRounds",
                table: "TournamentRounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Games",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Tournaments");

            migrationBuilder.RenameTable(
                name: "TournamentRounds",
                newName: "TournamentRound");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "Game");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentRounds_TournamentId",
                table: "TournamentRound",
                newName: "IX_TournamentRound_TournamentId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_TournamentRoundId",
                table: "Game",
                newName: "IX_Game_TournamentRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_Player2Id",
                table: "Game",
                newName: "IX_Game_Player2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Games_Player1Id",
                table: "Game",
                newName: "IX_Game_Player1Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentRound",
                table: "TournamentRound",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Game",
                table: "Game",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentPlayers_Player1Id",
                table: "Game",
                column: "Player1Id",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentPlayers_Player2Id",
                table: "Game",
                column: "Player2Id",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentRound_TournamentRoundId",
                table: "Game",
                column: "TournamentRoundId",
                principalTable: "TournamentRound",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentRound_Tournaments_TournamentId",
                table: "TournamentRound",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
