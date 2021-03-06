﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RomMaster.Client.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(nullable: false),
                    Size = table.Column<uint>(nullable: false),
                    Crc = table.Column<string>(nullable: true),
                    Sha1 = table.Column<string>(nullable: true),
                    Md5 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    FileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dat_File_FileId",
                        column: x => x.FileId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: true),
                    DatId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Dat_DatId",
                        column: x => x.DatId,
                        principalTable: "Dat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Size = table.Column<uint>(nullable: false),
                    Crc = table.Column<string>(nullable: true),
                    Sha1 = table.Column<string>(nullable: true),
                    Md5 = table.Column<string>(nullable: true),
                    GameId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rom_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dat_FileId",
                table: "Dat",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Dat_Name_Version",
                table: "Dat",
                columns: new[] { "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_Path",
                table: "File",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_DatId",
                table: "Game",
                column: "DatId");

            migrationBuilder.CreateIndex(
                name: "IX_Rom_GameId",
                table: "Rom",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rom");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Dat");

            migrationBuilder.DropTable(
                name: "File");
        }
    }
}
