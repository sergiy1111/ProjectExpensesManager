using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectExpensesManager.Migrations
{
    /// <inheritdoc />
    public partial class _7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForGoal",
                table: "Transactions");

            migrationBuilder.AddColumn<bool>(
                name: "ForGoal",
                table: "Categories",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForGoal",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "ForGoal",
                table: "Transactions",
                type: "bit",
                nullable: true);
        }
    }
}
