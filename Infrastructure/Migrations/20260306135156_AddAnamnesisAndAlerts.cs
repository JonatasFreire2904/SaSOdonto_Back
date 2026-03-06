using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnamnesisAndAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anamneses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HasDiabetes = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasHypertension = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasHeartDisease = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBleedingDisorder = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasHepatis = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasHiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasAsthma = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasSeizures = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSmoker = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAlcoholUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsesDrugs = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPregnant = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBreastfeeding = table.Column<bool>(type: "INTEGER", nullable: false),
                    Allergies = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Medications = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PreviousSurgeries = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FamilyHistory = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Observations = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anamneses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anamneses_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anamneses_PatientId",
                table: "Anamneses",
                column: "PatientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anamneses");
        }
    }
}
