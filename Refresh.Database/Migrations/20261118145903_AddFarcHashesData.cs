using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Refresh.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20261118145903_AddFarcHashesData")]
    public partial class AddFarcHashesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarcHashesData",
                columns: table => new
                {
                    Version = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GeneratedUsingJobVersion = table.Column<int>(type: "integer", nullable: false),
                    DlcHeaderHash = table.Column<string>(type: "text", nullable: false),
                    PinFarcHash = table.Column<string>(type: "text", nullable: false),
                    PackFarcHash = table.Column<string>(type: "text", nullable: false),
                    PinResourceHash = table.Column<string>(type: "text", nullable: false),
                    PackResourceHash = table.Column<string>(type: "text", nullable: false),
                    MapHash = table.Column<string>(type: "text", nullable: false),
                    EnglishTransTableHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarcHashesData", x => x.Version);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarcHashesData");
        }
    }
}
