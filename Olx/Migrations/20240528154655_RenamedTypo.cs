using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olx.Migrations
{
    /// <inheritdoc />
    public partial class RenamedTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReciverPatronymyc",
                table: "Orders",
                newName: "ReceiverPatronymyc");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ReceiverPatronymyc",
                table: "Orders",
                newName: "ReciverPatronymyc");
        }
    }
}
