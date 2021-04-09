using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBot.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolvewayUserId = table.Column<int>(type: "int", nullable: true),
                    TelegramUserId = table.Column<int>(type: "int", nullable: false),
                    StudyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyBearer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BearerExpiresIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SolvewayUserId",
                table: "Users",
                column: "SolvewayUserId",
                unique: true,
                filter: "[SolvewayUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                column: "TelegramUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
