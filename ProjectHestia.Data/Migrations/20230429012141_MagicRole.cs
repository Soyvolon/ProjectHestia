using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    /// <inheritdoc />
    public partial class MagicRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicRole",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SelectionSizeMin = table.Column<int>(type: "integer", nullable: false),
                    SelectionSizeMax = table.Column<int>(type: "integer", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LastInterval = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicRole", x => x.Key);
                    table.ForeignKey(
                        name: "FK_MagicRole_GuidConfigurations_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuidConfigurations",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MagicRole_GuildId",
                table: "MagicRole",
                column: "GuildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicRole");
        }
    }
}
