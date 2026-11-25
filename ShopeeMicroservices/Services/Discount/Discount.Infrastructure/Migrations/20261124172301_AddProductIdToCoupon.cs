using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discount.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "Coupons",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Backfill the seed coupons of databases created before this migration (no-op on a new database).
            migrationBuilder.Sql("""
                UPDATE "Coupons" SET "ProductId" = '019a7e00-0000-7000-8000-000000000301' WHERE "SkuCode" = 'IPHONE-X-SG-64';
                UPDATE "Coupons" SET "ProductId" = '019a7e00-0000-7000-8000-000000000308' WHERE "SkuCode" = 'IPHONE-17-BK-256';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Coupons");
        }
    }
}
