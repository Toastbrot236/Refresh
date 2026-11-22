using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refresh.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20261122221011_MoveScorePlayersIntoSeparateTable")]
    public partial class MoveScorePlayersIntoSeparateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameScorePlayers",
                columns: table => new
                {
                    ScoreId = table.Column<string>(type: "text", nullable: false),
                    PlayerNumber = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScorePlayers", x => new { x.ScoreId, x.PlayerNumber });
                    table.ForeignKey(
                        name: "FK_GameScorePlayers_GameScores_ScoreId",
                        column: x => x.ScoreId,
                        principalTable: "GameScores",
                        principalColumn: "ScoreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScorePlayers_GameUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "GameUsers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameScorePlayers_UserId",
                table: "GameScorePlayers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameScorePlayers");
        }
    }
}
