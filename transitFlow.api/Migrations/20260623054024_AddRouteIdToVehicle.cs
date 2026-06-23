using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transitFlow.api.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteIdToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Vehicles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Vehicles");
        }
    }
}
