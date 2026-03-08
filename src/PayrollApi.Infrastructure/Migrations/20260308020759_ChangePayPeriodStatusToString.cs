using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePayPeriodStatusToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PayrollPeriods",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Convert existing integer status values to string names
            migrationBuilder.Sql(@"
                UPDATE PayrollPeriods SET Status =
                    CASE Status
                        WHEN '1' THEN 'Draft'
                        WHEN '2' THEN 'Open'
                        WHEN '3' THEN 'Computed'
                        WHEN '4' THEN 'Approved'
                        WHEN '5' THEN 'Locked'
                        WHEN '6' THEN 'Exported'
                        ELSE 'Draft'
                    END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PayrollPeriods",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
