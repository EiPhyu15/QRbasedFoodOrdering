using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRbasedFoodOrdering.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQRcodeintoOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCode",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCode",
                table: "Order");
        }
    }
}
