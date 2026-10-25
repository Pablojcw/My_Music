using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeMusicSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Musics_Artists_ArtistId",
                table: "Musics");

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "Musics",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Musics",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ChannelTitle",
                table: "Musics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Musics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Musics",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Musics_YouTubeVideoId",
                table: "Musics",
                column: "YouTubeVideoId",
                unique: true,
                filter: "\"YouTubeVideoId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Musics_Artists_ArtistId",
                table: "Musics",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "ArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Musics_Artists_ArtistId",
                table: "Musics");

            migrationBuilder.DropIndex(
                name: "IX_Musics_YouTubeVideoId",
                table: "Musics");

            migrationBuilder.DropColumn(
                name: "ChannelTitle",
                table: "Musics");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Musics");

            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Musics");

            migrationBuilder.AlterColumn<int>(
                name: "DurationSeconds",
                table: "Musics",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Musics",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Musics_Artists_ArtistId",
                table: "Musics",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
