﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LA_LIGA_REKREATIVO.Server.Migrations
{
    /// <inheritdoc />
    public partial class secondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                schema: "rec",
                table: "MatchPlayer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<char>(
                name: "Action",
                schema: "rec",
                table: "MatchPlayer",
                type: "character(1)",
                nullable: false,
                defaultValue: ' ');
        }
    }
}
