using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pepper.Cards.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRemoteIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiDeckId",
                table: "DeckStyles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemoteDeckId",
                table: "Cards",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiDeckId",
                table: "DeckStyles");

            migrationBuilder.DropColumn(
                name: "RemoteDeckId",
                table: "Cards");
        }
    }
}
