using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeletedIsActiveAndLogoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "rec",
                table: "Teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "rec",
                table: "Teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "rec",
                table: "Players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "rec",
                table: "Players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                schema: "rec",
                table: "Players",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "rec",
                table: "Leagues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "rec",
                table: "Leagues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                schema: "rec",
                table: "Leagues",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "rec",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "rec",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "rec",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "rec",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Picture",
                schema: "rec",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "rec",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "rec",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "Logo",
                schema: "rec",
                table: "Leagues");
        }
    }
}
