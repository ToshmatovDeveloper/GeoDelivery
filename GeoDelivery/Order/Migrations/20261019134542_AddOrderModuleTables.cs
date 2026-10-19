using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderModuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAddressText",
                schema: "order",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddressText",
                schema: "order",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
