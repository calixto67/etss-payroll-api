using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayPeriodStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollRecords_PayrollPeriods_PayrollPeriodId",
                table: "PayrollRecords");

            migrationBuilder.AlterColumn<string>(
                name: "PeriodCode",
                table: "PayrollPeriods",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PayrollPeriods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPeriods_PeriodCode",
                table: "PayrollPeriods",
                column: "PeriodCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollRecords_PayrollPeriods_PayrollPeriodId",
                table: "PayrollRecords",
                column: "PayrollPeriodId",
                principalTable: "PayrollPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollRecords_PayrollPeriods_PayrollPeriodId",
                table: "PayrollRecords");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPeriods_PeriodCode",
                table: "PayrollPeriods");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PayrollPeriods");

            migrationBuilder.AlterColumn<string>(
                name: "PeriodCode",
                table: "PayrollPeriods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollRecords_PayrollPeriods_PayrollPeriodId",
                table: "PayrollRecords",
                column: "PayrollPeriodId",
                principalTable: "PayrollPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
