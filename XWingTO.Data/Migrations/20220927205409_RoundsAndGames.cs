using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XWingTO.Data.Migrations
{
    public partial class RoundsAndGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TournamentRound",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRound", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRound_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1MissionPoints = table.Column<int>(type: "int", nullable: false),
                    Player2MissionPoints = table.Column<int>(type: "int", nullable: false),
                    Turns = table.Column<int>(type: "int", nullable: false),
                    OutOfTime = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_TournamentPlayers_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Game_TournamentPlayers_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Game_TournamentRound_TournamentRoundId",
                        column: x => x.TournamentRoundId,
                        principalTable: "TournamentRound",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_Player1Id",
                table: "Game",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Game_Player2Id",
                table: "Game",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentRoundId",
                table: "Game",
                column: "TournamentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRound_TournamentId",
                table: "TournamentRound",
                column: "TournamentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "TournamentRound");
        }
    }
}
