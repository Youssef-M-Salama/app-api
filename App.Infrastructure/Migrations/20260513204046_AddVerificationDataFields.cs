using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationDataFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessLicenseNumber",
                table: "DonorOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessLicenseUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CivilProtectionApprovalUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegisterUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CommercialRegistrationDate",
                table: "DonorOrganizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumber",
                table: "DonorOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentalApprovalUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadquartersAddress",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipContractUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCardUrl",
                table: "DonorOrganizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "DonorOrganizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedPersonName",
                table: "Charities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedPersonPosition",
                table: "Charities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoardMembersListUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BylawsUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DelegationDocumentUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundersListUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadquartersAddress",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadquartersProofUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationCertificateUrl",
                table: "Charities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Charities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Charities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessLicenseNumber",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "BusinessLicenseUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "CivilProtectionApprovalUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "CommercialRegisterUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationDate",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumber",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "EnvironmentalApprovalUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "HeadquartersAddress",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "OwnershipContractUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "TaxCardUrl",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "DonorOrganizations");

            migrationBuilder.DropColumn(
                name: "AuthorizedPersonName",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "AuthorizedPersonPosition",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "BoardMembersListUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "BylawsUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "DelegationDocumentUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "FoundersListUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "HeadquartersAddress",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "HeadquartersProofUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "RegistrationCertificateUrl",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Charities");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Charities");
        }
    }
}
