using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kava.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Attachments",
                type: "TEXT",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<ulong>(
                name: "Size",
                table: "Attachments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Attachments");
        }
    }
}
