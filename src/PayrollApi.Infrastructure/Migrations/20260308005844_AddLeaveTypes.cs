using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultDaysPerYear = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EntitlementBasis = table.Column<int>(type: "int", nullable: false),
                    TenureIncrementDays = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TenureMaxDays = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EligibleEmploymentTypes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccrualMethod = table.Column<int>(type: "int", nullable: false),
                    PayCategory = table.Column<int>(type: "int", nullable: false),
                    PaidPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    BalanceDeductionMode = table.Column<int>(type: "int", nullable: false),
                    CarryForwardPolicy = table.Column<int>(type: "int", nullable: false),
                    CarryForwardMaxDays = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MinimumNoticeDays = table.Column<int>(type: "int", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    Granularity = table.Column<int>(type: "int", nullable: false),
                    CountWeekendsAsLeave = table.Column<bool>(type: "bit", nullable: false),
                    CountHolidaysAsLeave = table.Column<bool>(type: "bit", nullable: false),
                    AllowCashConversion = table.Column<bool>(type: "bit", nullable: false),
                    MaxCashConversionDays = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    GenderRestriction = table.Column<int>(type: "int", nullable: true),
                    MinServiceMonths = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_Code",
                table: "LeaveTypes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_Name",
                table: "LeaveTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveTypes");
        }
    }
}
