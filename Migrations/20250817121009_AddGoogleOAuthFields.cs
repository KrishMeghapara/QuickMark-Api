using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quick_CommerceApiForEx.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleOAuthFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleName",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePicture",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleUser",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GoogleName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GooglePicture",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsGoogleUser",
                table: "User");
        }
    }
}
