using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CC_Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtAndNormalizedCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comics",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Events",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Series",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Stories",
                table: "Creators");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeCreated",
                table: "Creators",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CreatorComics",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Returned = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: false),
                    CollectionUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorComics", x => x.CreatorId);
                    table.ForeignKey(
                        name: "FK_CreatorComics_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorEvents",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Returned = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: false),
                    CollectionUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorEvents", x => x.CreatorId);
                    table.ForeignKey(
                        name: "FK_CreatorEvents_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorSeries",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Returned = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: false),
                    CollectionUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorSeries", x => x.CreatorId);
                    table.ForeignKey(
                        name: "FK_CreatorSeries_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorStories",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Returned = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: false),
                    CollectionUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorStories", x => x.CreatorId);
                    table.ForeignKey(
                        name: "FK_CreatorStories_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorComicsItems",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorComicsItems", x => new { x.CreatorId, x.Id });
                    table.ForeignKey(
                        name: "FK_CreatorComicsItems_CreatorComics_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "CreatorComics",
                        principalColumn: "CreatorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorEventsItems",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorEventsItems", x => new { x.CreatorId, x.Id });
                    table.ForeignKey(
                        name: "FK_CreatorEventsItems_CreatorEvents_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "CreatorEvents",
                        principalColumn: "CreatorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorSeriesItems",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorSeriesItems", x => new { x.CreatorId, x.Id });
                    table.ForeignKey(
                        name: "FK_CreatorSeriesItems_CreatorSeries_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "CreatorSeries",
                        principalColumn: "CreatorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorStoriesItems",
                columns: table => new
                {
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ResourceUri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorStoriesItems", x => new { x.CreatorId, x.Id });
                    table.ForeignKey(
                        name: "FK_CreatorStoriesItems_CreatorStories_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "CreatorStories",
                        principalColumn: "CreatorId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreatorComicsItems");

            migrationBuilder.DropTable(
                name: "CreatorEventsItems");

            migrationBuilder.DropTable(
                name: "CreatorSeriesItems");

            migrationBuilder.DropTable(
                name: "CreatorStoriesItems");

            migrationBuilder.DropTable(
                name: "CreatorComics");

            migrationBuilder.DropTable(
                name: "CreatorEvents");

            migrationBuilder.DropTable(
                name: "CreatorSeries");

            migrationBuilder.DropTable(
                name: "CreatorStories");

            migrationBuilder.DropColumn(
                name: "DateTimeCreated",
                table: "Creators");

            migrationBuilder.AddColumn<string>(
                name: "Comics",
                table: "Creators",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Events",
                table: "Creators",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Series",
                table: "Creators",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stories",
                table: "Creators",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
