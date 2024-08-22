using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BibleVerseId",
                table: "Tag",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerseTag_TagId",
                table: "VerseTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_BibleVerseId",
                table: "Tag",
                column: "BibleVerseId");

            migrationBuilder.CreateIndex(
                name: "IX_BibleVerse_BibleBookId",
                table: "BibleVerse",
                column: "BibleBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BibleVerse_BibleBook_BibleBookId",
                table: "BibleVerse",
                column: "BibleBookId",
                principalTable: "BibleBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_BibleVerse_BibleVerseId",
                table: "Tag",
                column: "BibleVerseId",
                principalTable: "BibleVerse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VerseTag_Tag_TagId",
                table: "VerseTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BibleVerse_BibleBook_BibleBookId",
                table: "BibleVerse");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_BibleVerse_BibleVerseId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_VerseTag_Tag_TagId",
                table: "VerseTag");

            migrationBuilder.DropIndex(
                name: "IX_VerseTag_TagId",
                table: "VerseTag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_BibleVerseId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_BibleVerse_BibleBookId",
                table: "BibleVerse");

            migrationBuilder.DropColumn(
                name: "BibleVerseId",
                table: "Tag");
        }
    }
}
