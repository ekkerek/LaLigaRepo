using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    /// <inheritdoc />
    public partial class addedOficcialResultAndMatchSuspenedOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayTeamNegativePoints",
                schema: "rec",
                table: "Matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeTeamNegativePoints",
                schema: "rec",
                table: "Matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchSuspended",
                schema: "rec",
                table: "Matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficialResult",
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
                name: "AwayTeamNegativePoints",
                schema: "rec",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HomeTeamNegativePoints",
                schema: "rec",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsMatchSuspended",
                schema: "rec",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "IsOfficialResult",
                schema: "rec",
                table: "Matches");
        }
    }
}
