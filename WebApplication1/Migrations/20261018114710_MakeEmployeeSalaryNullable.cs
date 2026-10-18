using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmployeeSalaryNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "salary",
                table: "Employees",
                type: "float(7)",
                precision: 7,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(7)",
                oldPrecision: 7,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "salary",
                table: "Employees",
                type: "float(7)",
                precision: 7,
                scale: 2,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float(7)",
                oldPrecision: 7,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
