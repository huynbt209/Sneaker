using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sneaker.Data.Migrations
{
    public partial class AddFeedbackTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedbackProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackProducts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackProducts_ProductId",
                table: "FeedbackProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackProducts_UserId",
                table: "FeedbackProducts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackProducts");
        }
    }
}
