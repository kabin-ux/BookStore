using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiscountDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnSale",
                table: "Discounts");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Discounts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Discounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnSale",
                table: "Discounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
