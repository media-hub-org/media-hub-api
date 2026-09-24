using MediaHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<MusicTrack> MusicTracks => Set<MusicTrack>();
    public DbSet<MusicTag> MusicTags => Set<MusicTag>();
    public DbSet<MusicTrackTag> MusicTrackTags => Set<MusicTrackTag>();
    public DbSet<MusicPlaylist> MusicPlaylists => Set<MusicPlaylist>();
    public DbSet<MusicPlaylistTrack> MusicPlaylistTracks => Set<MusicPlaylistTrack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("devices");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.TokenHash).IsRequired();
            entity.HasIndex(d => d.TokenHash).IsUnique();
        });

        modelBuilder.Entity<MusicTrack>(entity =>
        {
            entity.ToTable("music_tracks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(300);
            entity.Property(t => t.Artist).HasMaxLength(200);
            entity.Property(t => t.Album).HasMaxLength(200);
            entity.Property(t => t.Genre).HasMaxLength(100);
            entity.Property(t => t.SourceName).IsRequired().HasMaxLength(50);
            entity.Property(t => t.StorageKey).IsRequired();
        });

        modelBuilder.Entity<MusicTag>(entity =>
        {
            entity.ToTable("music_tags");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(t => t.Name).IsUnique();
        });

        modelBuilder.Entity<MusicTrackTag>(entity =>
        {
            entity.ToTable("music_track_tags");
            entity.HasKey(tt => new { tt.MusicTrackId, tt.MusicTagId });

            entity.HasOne(tt => tt.MusicTrack)
                .WithMany()
                .HasForeignKey(tt => tt.MusicTrackId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tt => tt.MusicTag)
                .WithMany()
                .HasForeignKey(tt => tt.MusicTagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MusicPlaylist>(entity =>
        {
            entity.ToTable("music_playlists");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<MusicPlaylistTrack>(entity =>
        {
            entity.ToTable("music_playlist_tracks");
            entity.HasKey(pt => new { pt.MusicPlaylistId, pt.MusicTrackId });

            entity.HasOne(pt => pt.MusicPlaylist)
                .WithMany()
                .HasForeignKey(pt => pt.MusicPlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pt => pt.MusicTrack)
                .WithMany()
                .HasForeignKey(pt => pt.MusicTrackId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}