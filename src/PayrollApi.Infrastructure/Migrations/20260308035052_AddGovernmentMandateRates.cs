using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGovernmentMandateRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PagIbigContribution",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 100m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhilHealthRate",
                table: "Employees",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 2.5m);

            migrationBuilder.AddColumn<decimal>(
                name: "SssRate",
                table: "Employees",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 4.5m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPagIbigContribution",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPhilHealthRate",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSssRate",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PagIbigContribution",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhilHealthRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SssRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DefaultPagIbigContribution",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultPhilHealthRate",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultSssRate",
                table: "CompanySettings");
        }
    }
}
