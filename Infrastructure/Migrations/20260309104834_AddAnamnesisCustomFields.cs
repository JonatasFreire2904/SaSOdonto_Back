using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnamnesisCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnamnesisCustomFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Question = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Answer = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AnamnesisId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnamnesisCustomFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnamnesisCustomFields_Anamneses_AnamnesisId",
                        column: x => x.AnamnesisId,
                        principalTable: "Anamneses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesisCustomFields_AnamnesisId",
                table: "AnamnesisCustomFields",
                column: "AnamnesisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnamnesisCustomFields");
        }
    }
}
