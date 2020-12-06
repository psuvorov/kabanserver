﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kaban.UI.Migrations
{
    public partial class IntroduceICanBeArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                table: "Lists",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                table: "Cards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Lists");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Cards");
        }
    }
}
