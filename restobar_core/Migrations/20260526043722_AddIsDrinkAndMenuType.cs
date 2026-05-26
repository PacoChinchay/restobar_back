using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restobar_core.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDrinkAndMenuType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "menus",
                type: "text",
                nullable: false,
                defaultValue: "daily");

            migrationBuilder.AddColumn<bool>(
                name: "IsDrink",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "menus");

            migrationBuilder.DropColumn(
                name: "IsDrink",
                table: "categories");
        }
    }
}
