using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnstoenums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Approved, Rejected, Expired, Fulfilled");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "food, clothing, medical, education, etc");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OfferApplications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Accepted, Rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NeedApplications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Accepted, Rejected");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityNeeds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending",
                oldComment: "Pending, Approved, Rejected, Fulfilled");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityNeeds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Normal",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Normal",
                oldComment: "Urgent, High, Normal, Low");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "CharityNeeds",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "food, clothing, medical, education, etc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Offers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Approved, Rejected, Expired, Fulfilled",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Offers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "food, clothing, medical, education, etc",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OfferApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Accepted, Rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NeedApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Accepted, Rejected",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                comment: "Pending, Approved, Rejected, Fulfilled",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "CharityNeeds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Normal",
                comment: "Urgent, High, Normal, Low",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Normal");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "CharityNeeds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "food, clothing, medical, education, etc",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
