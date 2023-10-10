using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientAgent",
                table: "PgLoggers");

            migrationBuilder.DropColumn(
                name: "ClientIP",
                table: "PgLoggers");

            migrationBuilder.DropColumn(
                name: "LogEvent",
                table: "PgLoggers");

            migrationBuilder.DropColumn(
                name: "MessageTemplate",
                table: "PgLoggers");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "PgLoggers",
                newName: "Template");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Template",
                table: "PgLoggers",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "ClientAgent",
                table: "PgLoggers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientIP",
                table: "PgLoggers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogEvent",
                table: "PgLoggers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageTemplate",
                table: "PgLoggers",
                type: "text",
                nullable: true);
        }
    }
}
