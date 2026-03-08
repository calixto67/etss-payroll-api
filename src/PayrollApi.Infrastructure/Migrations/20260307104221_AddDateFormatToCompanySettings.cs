using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDateFormatToCompanySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFormat",
                table: "CompanySettings");
        }
    }
}
