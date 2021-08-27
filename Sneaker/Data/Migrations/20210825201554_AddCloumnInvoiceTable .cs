using Microsoft.EntityFrameworkCore.Migrations;

namespace Sneaker.Data.Migrations
{
    public partial class AddCloumnCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartItemId",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Carts");
        }
    }
}
