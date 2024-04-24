using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeleteProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "rec",
                table: "Summaries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "rec",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "rec",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "rec",
                table: "Matches");
        }
    }
}
