using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quick_CommerceApiForEx.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAddressRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_AddressID",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserID",
                table: "Address",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_User_UserID",
                table: "Address",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_User_UserID",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserID",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_User_AddressID",
                table: "User",
                column: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Address_AddressID",
                table: "User",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "AddressID");
        }
    }
}
