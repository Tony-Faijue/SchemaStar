using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedNodeAssetWithBlobNameAndNodeAssetName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "node_asset",
                type: "varchar(127)",
                maxLength: 127,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(127)",
                oldMaxLength: 127);

            migrationBuilder.AddColumn<string>(
                name: "blob_path",
                table: "node_asset",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "node_asset_name",
                table: "node_asset",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blob_path",
                table: "node_asset");

            migrationBuilder.DropColumn(
                name: "node_asset_name",
                table: "node_asset");

            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "node_asset",
                type: "varchar(127)",
                maxLength: 127,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(127)",
                oldMaxLength: 127,
                oldNullable: true);
        }
    }
}
