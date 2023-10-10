using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MySql.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TimestampLong",
                table: "Loggers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Loggers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_UserId",
                table: "Loggers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loggers_Users_UserId",
                table: "Loggers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loggers_Users_UserId",
                table: "Loggers");

            migrationBuilder.DropIndex(
                name: "IX_Loggers_UserId",
                table: "Loggers");

            migrationBuilder.DropColumn(
                name: "TimestampLong",
                table: "Loggers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Loggers");
        }
    }
}
