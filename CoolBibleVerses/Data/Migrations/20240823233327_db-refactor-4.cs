using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Migrations
{
    /// <inheritdoc />
    public partial class dbrefactor4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VerseTag",
                table: "VerseTag");

            migrationBuilder.DropIndex(
                name: "IX_VerseTag_TagId",
                table: "VerseTag");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VerseTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerseTag",
                table: "VerseTag",
                columns: new[] { "TagId", "BibleVerseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VerseTag",
                table: "VerseTag");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "VerseTag",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerseTag",
                table: "VerseTag",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VerseTag_TagId",
                table: "VerseTag",
                column: "TagId");
        }
    }
}
