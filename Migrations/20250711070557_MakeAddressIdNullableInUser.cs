using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quick_CommerceApiForEx.Migrations
{
    /// <inheritdoc />
    public partial class MakeAddressIdNullableInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "User",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "AddressID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "AddressID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
