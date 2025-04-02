using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTube.Migrations
{
    /// <inheritdoc />
    public partial class ThumbnailPathAddToVideoPostingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "VideoPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "VideoPostings");
        }
    }
}
