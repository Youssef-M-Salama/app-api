using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFulfillmentToApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FulfillmentDate",
                table: "OfferApplications",
                type: "datetime2",
                nullable: true,
                comment: "Set when the charity marks the accepted offer application as fulfilled");

            migrationBuilder.AddColumn<DateTime>(
                name: "FulfillmentDate",
                table: "NeedApplications",
                type: "datetime2",
                nullable: true,
                comment: "Set when the donor marks the accepted need application as fulfilled");

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_FulfillmentDate",
                table: "OfferApplications",
                column: "FulfillmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_FulfillmentDate",
                table: "NeedApplications",
                column: "FulfillmentDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OfferApplications_FulfillmentDate",
                table: "OfferApplications");

            migrationBuilder.DropIndex(
                name: "IX_NeedApplications_FulfillmentDate",
                table: "NeedApplications");

            migrationBuilder.DropColumn(
                name: "FulfillmentDate",
                table: "OfferApplications");

            migrationBuilder.DropColumn(
                name: "FulfillmentDate",
                table: "NeedApplications");
        }
    }
}
