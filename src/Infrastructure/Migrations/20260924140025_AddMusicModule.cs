using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "music_playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_playlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "music_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "music_tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Artist = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Album = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Genre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    SourceName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SourceUrl = table.Column<string>(type: "text", nullable: true),
                    StorageKey = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_tracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "music_playlist_tracks",
                columns: table => new
                {
                    MusicPlaylistId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusicTrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_playlist_tracks", x => new { x.MusicPlaylistId, x.MusicTrackId });
                    table.ForeignKey(
                        name: "FK_music_playlist_tracks_music_playlists_MusicPlaylistId",
                        column: x => x.MusicPlaylistId,
                        principalTable: "music_playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_music_playlist_tracks_music_tracks_MusicTrackId",
                        column: x => x.MusicTrackId,
                        principalTable: "music_tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "music_track_tags",
                columns: table => new
                {
                    MusicTrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusicTagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_track_tags", x => new { x.MusicTrackId, x.MusicTagId });
                    table.ForeignKey(
                        name: "FK_music_track_tags_music_tags_MusicTagId",
                        column: x => x.MusicTagId,
                        principalTable: "music_tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_music_track_tags_music_tracks_MusicTrackId",
                        column: x => x.MusicTrackId,
                        principalTable: "music_tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_music_playlist_tracks_MusicTrackId",
                table: "music_playlist_tracks",
                column: "MusicTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_music_tags_Name",
                table: "music_tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_music_track_tags_MusicTagId",
                table: "music_track_tags",
                column: "MusicTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "music_playlist_tracks");

            migrationBuilder.DropTable(
                name: "music_track_tags");

            migrationBuilder.DropTable(
                name: "music_playlists");

            migrationBuilder.DropTable(
                name: "music_tags");

            migrationBuilder.DropTable(
                name: "music_tracks");
        }
    }
}
