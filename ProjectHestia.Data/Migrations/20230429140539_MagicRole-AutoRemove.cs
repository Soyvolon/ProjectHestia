using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    /// <inheritdoc />
    public partial class MagicRoleAutoRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MaxMessages",
                table: "MagicRole",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal[]>(
                name: "WatchedChannels",
                table: "MagicRole",
                type: "numeric(20,0)[]",
                nullable: false,
                defaultValue: new decimal[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxMessages",
                table: "MagicRole");

            migrationBuilder.DropColumn(
                name: "WatchedChannels",
                table: "MagicRole");
        }
    }
}
