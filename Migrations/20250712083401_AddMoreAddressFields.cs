using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quick_CommerceApiForEx.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "House",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Landmark",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "House",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Landmark",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Address");
        }
    }
}
