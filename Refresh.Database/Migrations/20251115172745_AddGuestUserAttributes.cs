using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refresh.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GameDatabaseContext))]
    [Migration("20251115172745_AddGuestUserAttributes")]
    public partial class AddGuestUserAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastGameContactDate",
                table: "GameUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTimeOffset.MinValue);

            migrationBuilder.Sql("UPDATE \"GameUsers\" SET \"LastGameContactDate\" = \"LastLoginDate\"");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationCode",
                table: "GameUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGameContactDate",
                table: "GameUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationCode",
                table: "GameUsers");
        }
    }
}
