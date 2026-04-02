using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaySlip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeFixedAllowances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFixedAllowances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeFixedAllowances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalarySlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BaseSalary_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseSalary_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    OvertimeHours = table.Column<double>(type: "float", nullable: false),
                    OvertimeAmount_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeAmount_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    HolidayWorkDays = table.Column<int>(type: "int", nullable: false),
                    HolidayWorkAmount_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HolidayWorkAmount_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Bonuses_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonuses_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AbsentDays = table.Column<int>(type: "int", nullable: false),
                    AbsenceDeduction_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AbsenceDeduction_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    LateDeduction_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateDeduction_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    InsuranceDeduction_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceDeduction_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxDeduction_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxDeduction_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySlips_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalarySlipAllowances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalarySlipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySlipAllowances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySlipAllowances_SalarySlips_SalarySlipId",
                        column: x => x.SalarySlipId,
                        principalTable: "SalarySlips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFixedAllowances_EmployeeId",
                table: "EmployeeFixedAllowances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipAllowances_SalarySlipId",
                table: "SalarySlipAllowances",
                column: "SalarySlipId");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlips_EmployeeId_Month_Year",
                table: "SalarySlips",
                columns: new[] { "EmployeeId", "Month", "Year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeFixedAllowances");

            migrationBuilder.DropTable(
                name: "SalarySlipAllowances");

            migrationBuilder.DropTable(
                name: "SalarySlips");
        }
    }
}
