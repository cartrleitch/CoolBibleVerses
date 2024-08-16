using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolBibleVerses.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVerseTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "BibleVerse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "VerseTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BibleVerseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerseTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerseTag_BibleVerse_BibleVerseId",
                        column: x => x.BibleVerseId,
                        principalTable: "BibleVerse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerseTag_BibleVerseId",
                table: "VerseTag",
                column: "BibleVerseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerseTag");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "BibleVerse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
