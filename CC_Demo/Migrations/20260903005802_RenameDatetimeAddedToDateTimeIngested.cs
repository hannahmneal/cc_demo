using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC_Demo.Migrations
{
    /// <inheritdoc />
    public partial class RenameDatetimeAddedToDateTimeIngested : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DatetimeAdded",
                table: "Creators",
                newName: "DateTimeIngested");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTimeIngested",
                table: "Creators",
                newName: "DatetimeAdded");
        }
    }
}
