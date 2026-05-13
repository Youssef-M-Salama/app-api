using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVerificationStateToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DonorOrganizations_IsVerified",
                table: "DonorOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_Charities_IsVerified",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Charities");

            migrationBuilder.AddColumn<string>(
                name: "VerificationState",
                table: "DonorOrganizations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                comment: "Admin verification status");

            migrationBuilder.AddColumn<string>(
                name: "VerificationState",
                table: "Charities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                comment: "Admin verification status");

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_VerificationState",
                table: "DonorOrganizations",
                column: "VerificationState");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_VerificationState",
                table: "Charities",
                column: "VerificationState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DonorOrganizations_VerificationState",
                table: "DonorOrganizations");

            migrationBuilder.DropIndex(
                name: "IX_Charities_VerificationState",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "VerificationState",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "VerificationState",
                table: "Charities");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "DonorOrganizations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Admin verification status");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Charities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Admin verification status");

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_IsVerified",
                table: "DonorOrganizations",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_IsVerified",
                table: "Charities",
                column: "IsVerified");
        }
    }
}
