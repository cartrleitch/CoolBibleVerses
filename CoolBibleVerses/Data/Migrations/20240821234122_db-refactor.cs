using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "VerseTag");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "VerseTag",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagId",
                table: "VerseTag");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "VerseTag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
