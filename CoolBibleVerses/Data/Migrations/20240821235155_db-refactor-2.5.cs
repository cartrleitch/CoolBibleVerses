using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "bookId",
                table: "BibleVerse",
                newName: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BibleVerse",
                newName: "bookId");
        }
    }
}
