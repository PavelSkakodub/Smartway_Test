using Microsoft.EntityFrameworkCore.Migrations;

namespace Smartway_Test.Migrations
{
    public partial class EditLinksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Link_Files_FileId",
                table: "Link");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Link",
                table: "Link");

            migrationBuilder.RenameTable(
                name: "Link",
                newName: "Links");

            migrationBuilder.RenameIndex(
                name: "IX_Link_FileId",
                table: "Links",
                newName: "IX_Links_FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Links",
                table: "Links",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Files_FileId",
                table: "Links",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Files_FileId",
                table: "Links");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Links",
                table: "Links");

            migrationBuilder.RenameTable(
                name: "Links",
                newName: "Link");

            migrationBuilder.RenameIndex(
                name: "IX_Links_FileId",
                table: "Link",
                newName: "IX_Link_FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Link",
                table: "Link",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Link_Files_FileId",
                table: "Link",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
