using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor275 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BibleVerse",
                newName: "BibleBookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BibleBookId",
                table: "BibleVerse",
                newName: "BookId");
        }
    }
}
