using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olx.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_FavoredById",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_FavoredById",
                table: "Favorites",
                column: "FavoredById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_FavoredById",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_FavoredById",
                table: "Favorites",
                column: "FavoredById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
