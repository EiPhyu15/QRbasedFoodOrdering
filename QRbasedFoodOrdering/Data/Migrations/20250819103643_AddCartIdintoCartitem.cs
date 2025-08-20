using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRbasedFoodOrdering.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCartIdintoCartitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartId",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartId",
                table: "CartItem");
        }
    }
}
