using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatestatustype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DonorName",
                table: "DonorOrganizations",
                newName: "DonorOrganizationName");

            migrationBuilder.RenameColumn(
                name: "DonorId",
                table: "DonorOrganizations",
                newName: "DonorOrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_DonorOrganizations_DonorName",
                table: "DonorOrganizations",
                newName: "IX_DonorOrganizations_DonorOrganizationName");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiration",
                table: "Users",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Offers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Approved, Rejected, Expired, Fulfilled",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "available",
                oldComment: "available, expired");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Assigned when admin approves or rejects the offer",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Assigned when admin approves request");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OfferApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Accepted, Rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "pending",
                oldComment: "pending, accepted, rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NeedApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Accepted, Rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "pending",
                oldComment: "pending, accepted, rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Approved, Rejected, Fulfilled",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "pending",
                oldComment: "pending, approved, rejected, fulfilled");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Normal",
                comment: "Urgent, High, Normal, Low",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "normal",
                oldComment: "urgent, high, normal, low");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "CharityNeeds",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Assigned when admin approves or rejects the charity need",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Assigned when admin approves request");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DonorOrganizationName",
                table: "DonorOrganizations",
                newName: "DonorName");

            migrationBuilder.RenameColumn(
                name: "DonorOrganizationId",
                table: "DonorOrganizations",
                newName: "DonorId");

            migrationBuilder.RenameIndex(
                name: "IX_DonorOrganizations_DonorOrganizationName",
                table: "DonorOrganizations",
                newName: "IX_DonorOrganizations_DonorName");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiration",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Offers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "available",
                comment: "available, expired",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Approved, Rejected, Expired, Fulfilled");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Assigned when admin approves request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Assigned when admin approves or rejects the offer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OfferApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pending",
                comment: "pending, accepted, rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Accepted, Rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NeedApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pending",
                comment: "pending, accepted, rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Accepted, Rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pending",
                comment: "pending, approved, rejected, fulfilled",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Approved, Rejected, Fulfilled");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "normal",
                comment: "urgent, high, normal, low",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Normal",
                oldComment: "Urgent, High, Normal, Low");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "CharityNeeds",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Assigned when admin approves request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Assigned when admin approves or rejects the charity need");
        }
    }
}
