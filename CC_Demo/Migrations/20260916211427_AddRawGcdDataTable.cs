using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC_Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddRawGcdDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Raw_GCD_Data",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    gcd_id = table.Column<int>(type: "integer", nullable: false),
                    resource = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    datetime_ingested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raw_GCD_Data", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Raw_GCD_Data_resource_gcd_id",
                table: "Raw_GCD_Data",
                columns: new[] { "resource", "gcd_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Raw_GCD_Data");
        }
    }
}
