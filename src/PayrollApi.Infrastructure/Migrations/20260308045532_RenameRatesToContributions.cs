using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRatesToContributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhilHealthRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SssRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DefaultPhilHealthRate",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultSssRate",
                table: "CompanySettings");

            migrationBuilder.AlterColumn<decimal>(
                name: "PagIbigContribution",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 200m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 100m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhilHealthContribution",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 500m);

            migrationBuilder.AddColumn<decimal>(
                name: "SssContribution",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 900m);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirNo",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStarted",
                table: "CompanySettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPhilHealthContribution",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSssContribution",
                table: "CompanySettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EmployerSssNo",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndustryClassification",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNo",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhilHealthContribution",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SssContribution",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "BirNo",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DateStarted",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultPhilHealthContribution",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "DefaultSssContribution",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "EmployerSssNo",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "IndustryClassification",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "TaxNo",
                table: "CompanySettings");

            migrationBuilder.AlterColumn<decimal>(
                name: "PagIbigContribution",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 100m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 200m);

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
                name: "DefaultPhilHealthRate",
                table: "CompanySettings",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSssRate",
                table: "CompanySettings",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
