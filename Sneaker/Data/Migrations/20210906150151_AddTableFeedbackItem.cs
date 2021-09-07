using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sneaker.Data.Migrations
{
    public partial class AddTableFeedbackItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemsId",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FeedbackItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_ItemId",
                table: "InvoiceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ItemsId",
                table: "Carts",
                column: "ItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_ItemId",
                table: "FeedbackItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_UserId",
                table: "FeedbackItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Items_ItemsId",
                table: "Carts",
                column: "ItemsId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Items_ItemId",
                table: "InvoiceDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Items_ItemsId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Items_ItemId",
                table: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "FeedbackItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_ItemId",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ItemsId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ItemsId",
                table: "Carts");
        }
    }
}
