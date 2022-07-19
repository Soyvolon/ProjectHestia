using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    public partial class ModerationTools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStrikes",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStrikes", x => x.Key);
                    table.ForeignKey(
                        name: "FK_UserStrikes_GuidConfigurations_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuidConfigurations",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStrikes_GuildId",
                table: "UserStrikes",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStrikes");
        }
    }
}
