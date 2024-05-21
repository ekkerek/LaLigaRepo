using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    /// <inheritdoc />
    public partial class changedDoubleToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Coefficient",
                schema: "rec",
                table: "Leagues",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Coefficient",
                schema: "rec",
                table: "Leagues",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
