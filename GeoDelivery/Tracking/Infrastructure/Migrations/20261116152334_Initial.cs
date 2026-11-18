using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracking.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourierLocationPoints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    courier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courier_location_points", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_courier_location_points_courier_id",
                table: "CourierLocationPoints",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "ix_courier_location_points_order_id",
                table: "CourierLocationPoints",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_courier_location_points_timestamp",
                table: "CourierLocationPoints",
                column: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourierLocationPoints");
        }
    }
}
