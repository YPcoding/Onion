using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class _3st : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loggers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    MessageTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    Level = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Exception = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    ClientIP = table.Column<string>(type: "TEXT", nullable: true),
                    ClientAgent = table.Column<string>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: true),
                    LogEvent = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loggers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loggers");
        }
    }
}
