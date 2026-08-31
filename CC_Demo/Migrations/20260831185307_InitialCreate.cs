using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CC_Demo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MarvelId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Suffix = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Thumbnail = table.Column<string>(type: "jsonb", nullable: false),
                    AttributionHtml = table.Column<string>(type: "text", nullable: false),
                    AttributionText = table.Column<string>(type: "text", nullable: false),
                    Copyright = table.Column<string>(type: "text", nullable: false),
                    DatetimeAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Resource = table.Column<string>(type: "text", nullable: false),
                    ResourceUri = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Comics = table.Column<string>(type: "jsonb", nullable: false),
                    Events = table.Column<string>(type: "jsonb", nullable: false),
                    Series = table.Column<string>(type: "jsonb", nullable: false),
                    Stories = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrlMarvel",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlMarvel", x => new { x.CreatorId, x.Id });
                    table.ForeignKey(
                        name: "FK_UrlMarvel_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlMarvel");

            migrationBuilder.DropTable(
                name: "Creators");
        }
    }
}
