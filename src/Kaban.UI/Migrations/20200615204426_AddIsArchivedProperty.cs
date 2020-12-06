﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Kaban.UI.Migrations
{
    public partial class AddIsArchivedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Lists",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Cards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Boards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Boards");
        }
    }
}
