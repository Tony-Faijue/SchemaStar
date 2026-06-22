using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class NodeAssetSoftDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "node_asset",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "node_asset",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_node_active",
                table: "node_asset",
                columns: new[] { "is_deleted", "node_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_node_active",
                table: "node_asset");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "node_asset");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "node_asset");
        }
    }
}
