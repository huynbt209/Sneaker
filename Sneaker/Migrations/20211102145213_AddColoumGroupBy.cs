using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sneaker.Migrations
{
    public partial class AddColoumGroupBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "OpenGroupBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "OrderItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ApplicationUserId",
                table: "OrderItems",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_AspNetUsers_ApplicationUserId",
                table: "OrderItems",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_AspNetUsers_ApplicationUserId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ApplicationUserId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OpenGroupBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "OrderItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
