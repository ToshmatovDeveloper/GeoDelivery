using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_restaurants_RestaurantId",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_restaurants",
                table: "restaurants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dishes",
                table: "dishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "restaurants",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "restaurants",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "restaurants",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "restaurants",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "dishes",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "dishes",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "dishes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "dishes",
                newName: "restaurant_id");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "dishes",
                newName: "is_available");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "dishes",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "categories",
                newName: "restaurant_id");

            migrationBuilder.RenameIndex(
                name: "IX_categories_RestaurantId",
                table: "categories",
                newName: "ix_categories_restaurant_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_restaurants",
                table: "restaurants",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dishes",
                table: "dishes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories",
                table: "categories",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_restaurants_restaurant_id",
                table: "categories",
                column: "restaurant_id",
                principalTable: "restaurants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_restaurants_restaurant_id",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_restaurants",
                table: "restaurants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dishes",
                table: "dishes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "restaurants",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "restaurants",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "restaurants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "restaurants",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "dishes",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "dishes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "dishes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "restaurant_id",
                table: "dishes",
                newName: "RestaurantId");

            migrationBuilder.RenameColumn(
                name: "is_available",
                table: "dishes",
                newName: "IsAvailable");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "dishes",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "restaurant_id",
                table: "categories",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "ix_categories_restaurant_id",
                table: "categories",
                newName: "IX_categories_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_restaurants",
                table: "restaurants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dishes",
                table: "dishes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_restaurants_RestaurantId",
                table: "categories",
                column: "RestaurantId",
                principalTable: "restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
