using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_BibleVerse_BibleVerseId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_BibleVerseId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "BibleVerseId",
                table: "Tag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BibleVerseId",
                table: "Tag",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_BibleVerseId",
                table: "Tag",
                column: "BibleVerseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_BibleVerse_BibleVerseId",
                table: "Tag",
                column: "BibleVerseId",
                principalTable: "BibleVerse",
                principalColumn: "Id");
        }
    }
}
