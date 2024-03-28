using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olx.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EnterpriseUser_EnterpriseUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_EnterpriseUser_UserId",
                table: "EnterpriseUser");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EnterpriseUserId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsPromoted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PromotionEnd",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseUser_UserId",
                table: "EnterpriseUser",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EnterpriseUser_UserId",
                table: "EnterpriseUser");

            migrationBuilder.DropColumn(
                name: "IsPromoted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PromotionEnd",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseUser_UserId",
                table: "EnterpriseUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EnterpriseUserId",
                table: "AspNetUsers",
                column: "EnterpriseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EnterpriseUser_EnterpriseUserId",
                table: "AspNetUsers",
                column: "EnterpriseUserId",
                principalTable: "EnterpriseUser",
                principalColumn: "Id");
        }
    }
}
