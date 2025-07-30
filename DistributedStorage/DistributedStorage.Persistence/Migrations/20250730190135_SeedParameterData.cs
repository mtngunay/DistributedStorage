using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DistributedStorage.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedParameterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "CreatedAt", "Key", "Value" },
                values: new object[,]
                {
                    { new Guid("21ac0ad6-49fa-41f8-bd76-ed29cafbc1d0"), new DateTime(2025, 7, 30, 21, 57, 46, 0, DateTimeKind.Utc), "AllowedMimeTypes", "application/pdf,image/png,image/jpeg" },
                    { new Guid("d9211026-d96c-4950-912e-ec2c48071b08"), new DateTime(2025, 7, 30, 21, 57, 46, 0, DateTimeKind.Utc), "DefaultChunkSizePercent", "1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: new Guid("21ac0ad6-49fa-41f8-bd76-ed29cafbc1d0"));

            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: new Guid("d9211026-d96c-4950-912e-ec2c48071b08"));
        }
    }
}