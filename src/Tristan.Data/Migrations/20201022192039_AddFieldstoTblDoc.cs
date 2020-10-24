using Microsoft.EntityFrameworkCore.Migrations;

namespace Tristan.Data.Migrations
{
    public partial class AddFieldstoTblDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Moved",
                table: "TblDoc",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRetry",
                table: "TblDoc",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moved",
                table: "TblDoc");

            migrationBuilder.DropColumn(
                name: "NumberOfRetry",
                table: "TblDoc");
        }
    }
}
