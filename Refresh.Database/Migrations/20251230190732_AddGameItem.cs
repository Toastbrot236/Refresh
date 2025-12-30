using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refresh.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20251230190732_AddGameItem")]
    public partial class AddGameItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameItems",
                columns: table => new
                {
                    PlanHash = table.Column<string>(type: "text", nullable: false),
                    IconHash = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatorName = table.Column<string>(type: "text", nullable: false),
                    ContributorNames = table.Column<string[]>(type: "text[]", nullable: false),
                    IsGamePhoto = table.Column<bool>(type: "boolean", nullable: false),
                    IsCameraPhoto = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreation = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameItems", x => x.PlanHash);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameItems");
        }
    }
}
