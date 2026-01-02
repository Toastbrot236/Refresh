using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refresh.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20260102131608_AddGameInnerLevel")]
    public partial class AddGameInnerLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameInnerLevels",
                columns: table => new
                {
                    InnerId = table.Column<int>(type: "integer", nullable: false),
                    AdventureId = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    MetadataUpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RootResourceUpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DiscoveredFromPublish = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IconHash = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LevelType = table.Column<byte>(type: "smallint", nullable: false),
                    MinPlayers = table.Column<int>(type: "integer", nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    EnforceMinMaxPlayers = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresMoveController = table.Column<bool>(type: "boolean", nullable: false),
                    RootResource = table.Column<string>(type: "text", nullable: false),
                    LocationX = table.Column<float>(type: "real", nullable: false),
                    LocationY = table.Column<float>(type: "real", nullable: false),
                    LocationZ = table.Column<float>(type: "real", nullable: false),
                    BadgeSize = table.Column<byte>(type: "smallint", nullable: false),
                    Labels = table.Column<byte[]>(type: "smallint[]", nullable: false),
                    ContributorNames = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsModded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInnerLevels", x => new { x.InnerId, x.AdventureId });
                    table.ForeignKey(
                        name: "FK_GameInnerLevels_GameLevels_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "GameLevels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInnerLevels_AdventureId",
                table: "GameInnerLevels",
                column: "AdventureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameInnerLevels");
        }
    }
}
