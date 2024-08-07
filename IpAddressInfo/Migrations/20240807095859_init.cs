using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IpAddressInfo.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TwoLetterCode = table.Column<string>(type: "text", nullable: false),
                    ThreeLetterCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IPAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedAt", "Name", "ThreeLetterCode", "TwoLetterCode" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Greece", "GRC", "GR" },
                    { 2, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Germany", "DEU", "DE" },
                    { 3, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Cyprus", "CYP", "CY" },
                    { 4, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "United States", "USA", "US" },
                    { 6, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Spain", "ESP", "ES" },
                    { 7, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "France", "FRA", "FR" },
                    { 8, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Italy", "ITA", "IT" },
                    { 9, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "Japan", "JPN", "JP" },
                    { 10, new DateTime(2022, 10, 12, 6, 46, 10, 500, DateTimeKind.Utc), "China", "CHN", "CN" }
                });

            migrationBuilder.InsertData(
                table: "IPAddresses",
                columns: new[] { "Id", "CountryId", "CreatedAt", "IP", "UpdatedAt" },
                values: new object[,]
                {
                    { 6, 1, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "44.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 7, 2, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "45.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 8, 3, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "46.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 9, 4, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "47.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 10, 6, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "49.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 11, 7, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "41.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 12, 8, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "42.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 13, 9, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "43.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 14, 10, new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660), "50.255.255.254", new DateTime(2022, 10, 12, 7, 4, 6, 856, DateTimeKind.Utc).AddTicks(6660) },
                    { 15, 1, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "44.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 16, 2, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "45.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 17, 3, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "46.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 18, 4, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "47.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 19, 6, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "49.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 20, 7, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "41.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 21, 8, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "42.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 22, 9, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "43.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 23, 10, new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc), "50.25.55.254", new DateTime(2022, 10, 12, 7, 4, 33, 380, DateTimeKind.Utc) },
                    { 24, 1, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "44.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 25, 2, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "45.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 26, 3, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "46.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 27, 4, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "47.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 28, 6, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "49.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 29, 7, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "41.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 30, 8, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "42.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 31, 9, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "43.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 32, 10, new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330), "50.25.55.4", new DateTime(2022, 10, 12, 7, 4, 51, 323, DateTimeKind.Utc).AddTicks(3330) },
                    { 33, 1, new DateTime(2022, 10, 12, 8, 41, 37, 310, DateTimeKind.Utc), "10.20.30.40", new DateTime(2022, 10, 12, 8, 41, 37, 310, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IPAddresses_CountryId",
                table: "IPAddresses",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPAddresses");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
