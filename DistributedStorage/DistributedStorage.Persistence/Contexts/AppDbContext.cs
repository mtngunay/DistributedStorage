using DistributedStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = DistributedStorage.Domain.Entities.File;

namespace DistributedStorage.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<File> Files => Set<File>();
        public DbSet<Chunk> Chunks => Set<Chunk>();
        public DbSet<ChunkBlob> ChunkBlobs => Set<ChunkBlob>();

        public DbSet<Parameter> Parameters => Set<Parameter>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.MimeType).IsRequired().HasMaxLength(128);
                entity.Property(e => e.Checksum).IsRequired().HasMaxLength(64);
                entity.Property(e => e.Size).IsRequired();
                entity.Property(e => e.ChunkCount).IsRequired();

                entity
                    .HasMany(e => e.Chunks)
                    .WithOne(c => c.File)
                    .HasForeignKey(c => c.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Chunk>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Checksum).IsRequired().HasMaxLength(64);
                entity.Property(e => e.Size).IsRequired();
                entity.Property(e => e.ChunkNumber).IsRequired();

                entity
                    .HasOne(e => e.ChunkBlob)
                    .WithOne(cb => cb.Chunk)
                    .HasForeignKey<ChunkBlob>(cb => cb.ChunkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChunkBlob>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Data).IsRequired();
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(500);

                entity.HasData(
                    new Parameter
                    {
                        Id = new Guid("d9211026-d96c-4950-912e-ec2c48071b08"),
                        Key = "DefaultChunkSizePercent",
                        Value = "1",
                        CreatedAt = new DateTime(2025, 7, 30, 21, 57, 46, DateTimeKind.Utc),
                    },
                    new Parameter
                    {
                        Id = new Guid("21ac0ad6-49fa-41f8-bd76-ed29cafbc1d0"),
                        Key = "AllowedMimeTypes",
                        Value = "application/pdf,image/png,image/jpeg",
                        CreatedAt = new DateTime(2025, 7, 30, 21, 57, 46, DateTimeKind.Utc),
                    }
                );
            });
        }
    }
}