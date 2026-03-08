using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCompanySettingsDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultSssRate",
                table: "CompanySettings",
                type: "decimal(6,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultPhilHealthRate",
                table: "CompanySettings",
                type: "decimal(6,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultSssRate",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultPhilHealthRate",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)");
        }
    }
}
