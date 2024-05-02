using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olx.Migrations
{
    /// <inheritdoc />
    public partial class AddedFavoriteProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoredById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FavoritesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.FavoredById, x.FavoritesId });
                    table.ForeignKey(
                        name: "FK_Favorites_AspNetUsers_FavoredById",
                        column: x => x.FavoredById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Products_FavoritesId",
                        column: x => x.FavoritesId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_FavoritesId",
                table: "Favorites",
                column: "FavoritesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");
        }
    }
}
