using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class banneranddiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "IsOnSale",
                table: "Discounts");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Discounts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                    table.ForeignKey(
                        name: "FK_Banners_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CreatedByUserId",
                table: "Banners",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Discounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnSale",
                table: "Discounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts",
                column: "BookId",
                unique: true);
        }
    }
}
