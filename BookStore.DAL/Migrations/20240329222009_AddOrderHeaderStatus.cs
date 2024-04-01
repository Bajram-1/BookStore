using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderHeaderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "StripePaymentId",
                table: "OrderHeaders");
        }
    }
}
