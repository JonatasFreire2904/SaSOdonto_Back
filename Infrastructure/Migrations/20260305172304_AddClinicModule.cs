using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "UserClinics",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserClinics",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Clinics",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Clinics",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Clinics",
                type: "TEXT",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Clinics",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Clinics",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_OwnerId",
                table: "Clinics",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_Users_OwnerId",
                table: "Clinics",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_Users_OwnerId",
                table: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_OwnerId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "UserClinics");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserClinics");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Clinics");
        }
    }
}
