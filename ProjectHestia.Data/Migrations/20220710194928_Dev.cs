using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    public partial class Dev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuidConfigurations",
                columns: table => new
                {
                    Key = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuidConfigurations", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "GuildQuotes",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    QuoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Author = table.Column<string>(type: "text", nullable: true),
                    SavedBy = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    ColorRaw = table.Column<int>(type: "integer", nullable: true),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Uses = table.Column<long>(type: "bigint", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildQuotes", x => x.Key);
                    table.UniqueConstraint("AK_GuildQuotes_GuildId_QuoteId", x => new { x.GuildId, x.QuoteId });
                    table.ForeignKey(
                        name: "FK_GuildQuotes_GuidConfigurations_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuidConfigurations",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildQuotes");

            migrationBuilder.DropTable(
                name: "GuidConfigurations");
        }
    }
}
