using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XWingTO.Data.Migrations
{
    public partial class AddPlayerIdsToGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TournamentPlayer1Id",
                table: "Games",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TournamentPlayer2Id",
                table: "Games",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentPlayer1Id",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TournamentPlayer2Id",
                table: "Games");
        }
    }
}
