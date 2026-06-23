using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transitFlow.api.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceNumberToStopRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "RouteStops",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "RouteStops");
        }
    }
}
