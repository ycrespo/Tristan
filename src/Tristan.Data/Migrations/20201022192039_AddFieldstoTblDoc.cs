using Microsoft.EntityFrameworkCore.Migrations;

namespace Tristan.Data.Migrations
{
    public partial class AddFieldstoDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Moved",
                table: "Doc",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRetry",
                table: "Doc",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moved",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "NumberOfRetry",
                table: "Doc");
        }
    }
}
