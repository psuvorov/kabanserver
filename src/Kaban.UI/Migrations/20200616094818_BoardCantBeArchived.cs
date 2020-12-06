﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Kaban.UI.Migrations
{
    public partial class BoardCantBeArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Boards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Boards",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
