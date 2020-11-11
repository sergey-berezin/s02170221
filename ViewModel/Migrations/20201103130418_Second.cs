using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ViewModel.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Types_PictureTypeDBId",
                table: "Pictures");

            migrationBuilder.RenameColumn(
                name: "PictureTypeDBId",
                table: "Pictures",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Pictures",
                newName: "ShortFileName");

            migrationBuilder.RenameIndex(
                name: "IX_Pictures_PictureTypeDBId",
                table: "Pictures",
                newName: "IX_Pictures_TypeId");

            migrationBuilder.AddColumn<int>(
                name: "PictureInfoDetailsId",
                table: "Pictures",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BinaryFile = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_PictureInfoDetailsId",
                table: "Pictures",
                column: "PictureInfoDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Details_PictureInfoDetailsId",
                table: "Pictures",
                column: "PictureInfoDetailsId",
                principalTable: "Details",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Types_TypeId",
                table: "Pictures",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Details_PictureInfoDetailsId",
                table: "Pictures");

            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Types_TypeId",
                table: "Pictures");

            migrationBuilder.DropTable(
                name: "Details");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_PictureInfoDetailsId",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "PictureInfoDetailsId",
                table: "Pictures");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Pictures",
                newName: "PictureTypeDBId");

            migrationBuilder.RenameColumn(
                name: "ShortFileName",
                table: "Pictures",
                newName: "FileName");

            migrationBuilder.RenameIndex(
                name: "IX_Pictures_TypeId",
                table: "Pictures",
                newName: "IX_Pictures_PictureTypeDBId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Types_PictureTypeDBId",
                table: "Pictures",
                column: "PictureTypeDBId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
