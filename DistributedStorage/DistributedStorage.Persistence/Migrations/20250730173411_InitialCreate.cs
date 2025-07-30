using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistributedStorage.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ChunkCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChunkNumber = table.Column<int>(type: "int", nullable: false),
                    Checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chunks_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChunkBlobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChunkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkBlobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChunkBlobs_Chunks_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "Chunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChunkBlobs_ChunkId",
                table: "ChunkBlobs",
                column: "ChunkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chunks_FileId",
                table: "Chunks",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkBlobs");

            migrationBuilder.DropTable(
                name: "Chunks");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}