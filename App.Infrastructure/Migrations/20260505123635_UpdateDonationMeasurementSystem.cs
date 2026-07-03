using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDonationMeasurementSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Offers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                comment: "Quantity available",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity available");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Piece");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "CharityNeeds",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                comment: "Quantity needed",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity needed");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "CharityNeeds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Piece");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "CharityNeeds");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Offers",
                type: "int",
                nullable: false,
                comment: "Quantity available",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComment: "Quantity available");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "CharityNeeds",
                type: "int",
                nullable: false,
                comment: "Quantity needed",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComment: "Quantity needed");
        }
    }
}
