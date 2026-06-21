using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class ImplementSoftDeletionNodeEdge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "node",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "node",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "edge",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "edge",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_edge_lookup",
                table: "edge",
                columns: new[] { "is_deleted", "from_node_id", "to_node_id" });

            migrationBuilder.CreateIndex(
                name: "ix_node_web_active",
                table: "node",
                columns: new[] { "is_deleted", "node_web_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_node_web_active",
                table: "node");

            migrationBuilder.DropIndex(
                name: "ix_edge_lookup",
                table: "edge");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "node");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "node");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "edge");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "edge");
        }
    }
}
