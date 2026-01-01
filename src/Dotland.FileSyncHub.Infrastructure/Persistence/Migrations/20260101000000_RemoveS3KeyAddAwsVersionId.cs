using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotland.FileSyncHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveS3KeyAddAwsVersionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "S3Key",
                table: "DocumentVersions");

            migrationBuilder.AddColumn<string>(
                name: "AwsVersionId",
                table: "DocumentVersions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsVersionId",
                table: "DocumentVersions");

            migrationBuilder.AddColumn<string>(
                name: "S3Key",
                table: "DocumentVersions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
