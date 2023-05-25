using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectHestia.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRandomRemovalChance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RandomRemovePercentageModPerMessage",
                table: "MagicRole",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RandomRemoveStartingPercentage",
                table: "MagicRole",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UsePercentBootInsteadOfMaxMessages",
                table: "MagicRole",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomRemovePercentageModPerMessage",
                table: "MagicRole");

            migrationBuilder.DropColumn(
                name: "RandomRemoveStartingPercentage",
                table: "MagicRole");

            migrationBuilder.DropColumn(
                name: "UsePercentBootInsteadOfMaxMessages",
                table: "MagicRole");
        }
    }
}
