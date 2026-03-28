using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class MoveToNodeAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "node_audio_mime",
                table: "node");

            migrationBuilder.DropColumn(
                name: "node_audio_size",
                table: "node");

            migrationBuilder.DropColumn(
                name: "node_audio_url",
                table: "node");

            migrationBuilder.DropColumn(
                name: "node_image_mime",
                table: "node");

            migrationBuilder.DropColumn(
                name: "node_image_size",
                table: "node");

            migrationBuilder.DropColumn(
                name: "node_image_url",
                table: "node");

            migrationBuilder.RenameIndex(
                name: "public_id3",
                table: "users",
                newName: "public_id4");

            migrationBuilder.RenameIndex(
                name: "public_id2",
                table: "nodeweb",
                newName: "public_id3");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "node",
                type: "timestamp",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "node",
                type: "enum('locked','pinned','unlocked')",
                nullable: false,
                defaultValueSql: "'unlocked'",
                oldClrType: typeof(string),
                oldType: "enum('locked','pinned','unlocked')",
                oldNullable: true,
                oldDefaultValueSql: "'unlocked'");

            migrationBuilder.AlterColumn<double>(
                name: "position_y",
                table: "node",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "position_x",
                table: "node",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "UserId",
                table: "node",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "node_asset",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: false),
                    asset_type = table.Column<string>(type: "enum('image', 'audio', 'video', 'link')", nullable: false, defaultValueSql: "'link'"),
                    asset_source = table.Column<string>(type: "enum('upload', 'external')", nullable: false, defaultValueSql: "'external'"),
                    url = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true),
                    mime_type = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: false),
                    file_size = table.Column<int>(type: "int", nullable: true),
                    node_id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_node_assets_node",
                        column: x => x.node_id,
                        principalTable: "node",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "node_id",
                table: "node_asset",
                column: "node_id");

            migrationBuilder.CreateIndex(
                name: "public_id2",
                table: "node_asset",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "node_asset");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "node");

            migrationBuilder.RenameIndex(
                name: "public_id4",
                table: "users",
                newName: "public_id3");

            migrationBuilder.RenameIndex(
                name: "public_id3",
                table: "nodeweb",
                newName: "public_id2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "node",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "node",
                type: "enum('locked','pinned','unlocked')",
                nullable: true,
                defaultValueSql: "'unlocked'",
                oldClrType: typeof(string),
                oldType: "enum('locked','pinned','unlocked')",
                oldDefaultValueSql: "'unlocked'");

            migrationBuilder.AlterColumn<double>(
                name: "position_y",
                table: "node",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "position_x",
                table: "node",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<string>(
                name: "node_audio_mime",
                table: "node",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "node_audio_size",
                table: "node",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "node_audio_url",
                table: "node",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "node_image_mime",
                table: "node",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "node_image_size",
                table: "node",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "node_image_url",
                table: "node",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
