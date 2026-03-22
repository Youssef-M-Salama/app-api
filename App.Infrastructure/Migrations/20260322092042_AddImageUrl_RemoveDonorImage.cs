using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrl_RemoveDonorImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonorOrganizationImage",
                table: "DonorOrganizations");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "DonorOrganizationImage",
                table: "DonorOrganizations",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }
    }
}
