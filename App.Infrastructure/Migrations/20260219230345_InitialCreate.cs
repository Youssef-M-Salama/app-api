using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Whatsapp = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Governorate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Charities",
                columns: table => new
                {
                    CharityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CharityDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Admin verification status"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charities", x => x.CharityId);
                    table.ForeignKey(
                        name: "FK_Charities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonorOrganizations",
                columns: table => new
                {
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonorOrganizationImage = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Admin verification status"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorOrganizations", x => x.DonorId);
                    table.ForeignKey(
                        name: "FK_DonorOrganizations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityNeeds",
                columns: table => new
                {
                    CharityNeedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Assigned when admin approves request"),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "food, clothing, medical, education, etc"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Quantity needed"),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "normal", comment: "urgent, high, normal, low"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending", comment: "pending, approved, rejected, fulfilled"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityNeeds", x => x.CharityNeedId);
                    table.ForeignKey(
                        name: "FK_CharityNeeds_Charities_CharityId",
                        column: x => x.CharityId,
                        principalTable: "Charities",
                        principalColumn: "CharityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Assigned when admin approves request"),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "food, clothing, medical, education, etc"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Quantity available"),
                    ProductImage = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When offer expires"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "available", comment: "available, expired"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.OfferId);
                    table.ForeignKey(
                        name: "FK_Offers_DonorOrganizations_DonorOrganizationId",
                        column: x => x.DonorOrganizationId,
                        principalTable: "DonorOrganizations",
                        principalColumn: "DonorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NeedApplications",
                columns: table => new
                {
                    NeedApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharityNeedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending", comment: "pending, accepted, rejected"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeedApplications", x => x.NeedApplicationId);
                    table.ForeignKey(
                        name: "FK_NeedApplications_CharityNeeds_CharityNeedId",
                        column: x => x.CharityNeedId,
                        principalTable: "CharityNeeds",
                        principalColumn: "CharityNeedId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NeedApplications_DonorOrganizations_DonorOrganizationId",
                        column: x => x.DonorOrganizationId,
                        principalTable: "DonorOrganizations",
                        principalColumn: "DonorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfferApplications",
                columns: table => new
                {
                    OfferApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending", comment: "pending, accepted, rejected"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferApplications", x => x.OfferApplicationId);
                    table.ForeignKey(
                        name: "FK_OfferApplications_Charities_CharityId",
                        column: x => x.CharityId,
                        principalTable: "Charities",
                        principalColumn: "CharityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferApplications_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "OfferId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_CharityName",
                table: "Charities",
                column: "CharityName");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_IsActive",
                table: "Charities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_IsVerified",
                table: "Charities",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Charities_UserId",
                table: "Charities",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_Category",
                table: "CharityNeeds",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_CharityId",
                table: "CharityNeeds",
                column: "CharityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_CreatedAt",
                table: "CharityNeeds",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_Priority",
                table: "CharityNeeds",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_Status",
                table: "CharityNeeds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CharityNeeds_Status_Category",
                table: "CharityNeeds",
                columns: new[] { "Status", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_DonorName",
                table: "DonorOrganizations",
                column: "DonorName");

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_IsActive",
                table: "DonorOrganizations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_IsVerified",
                table: "DonorOrganizations",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_DonorOrganizations_UserId",
                table: "DonorOrganizations",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_CharityNeedId",
                table: "NeedApplications",
                column: "CharityNeedId");

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_CharityNeedId_Status",
                table: "NeedApplications",
                columns: new[] { "CharityNeedId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_CreatedAt",
                table: "NeedApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_DonorOrganizationId",
                table: "NeedApplications",
                column: "DonorOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_DonorOrganizationId_Status",
                table: "NeedApplications",
                columns: new[] { "DonorOrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_Status",
                table: "NeedApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NeedApplications_Unique_DonorNeed",
                table: "NeedApplications",
                columns: new[] { "DonorOrganizationId", "CharityNeedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_CharityId",
                table: "OfferApplications",
                column: "CharityId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_CharityId_Status",
                table: "OfferApplications",
                columns: new[] { "CharityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_CreatedAt",
                table: "OfferApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_OfferId",
                table: "OfferApplications",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_OfferId_Status",
                table: "OfferApplications",
                columns: new[] { "OfferId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_Status",
                table: "OfferApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OfferApplications_Unique_CharityOffer",
                table: "OfferApplications",
                columns: new[] { "CharityId", "OfferId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_Category",
                table: "Offers",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CreatedAt",
                table: "Offers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_DonorOrganizationId",
                table: "Offers",
                column: "DonorOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ExpiryDate",
                table: "Offers",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_Status",
                table: "Offers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_Status_Category",
                table: "Offers",
                columns: new[] { "Status", "Category" });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NeedApplications");

            migrationBuilder.DropTable(
                name: "OfferApplications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CharityNeeds");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "Charities");

            migrationBuilder.DropTable(
                name: "DonorOrganizations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
