using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedEdgeWithNodeWebIdAndJsonMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "edge_ibfk_1",
                table: "edge");

            migrationBuilder.AddColumn<ulong>(
                name: "node_web_id",
                table: "edge",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "ui_metadata",
                table: "edge",
                type: "json",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_edge_node_web_id",
                table: "edge",
                column: "node_web_id");

            migrationBuilder.AddForeignKey(
                name: "FK_edge_nodeweb_node_web_id",
                table: "edge",
                column: "node_web_id",
                principalTable: "nodeweb",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "edge_ibfk_1",
                table: "edge",
                column: "from_node_id",
                principalTable: "node",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_edge_nodeweb_node_web_id",
                table: "edge");

            migrationBuilder.DropForeignKey(
                name: "edge_ibfk_1",
                table: "edge");

            migrationBuilder.DropIndex(
                name: "IX_edge_node_web_id",
                table: "edge");

            migrationBuilder.DropColumn(
                name: "node_web_id",
                table: "edge");

            migrationBuilder.DropColumn(
                name: "ui_metadata",
                table: "edge");

            migrationBuilder.AddForeignKey(
                name: "edge_ibfk_1",
                table: "edge",
                column: "from_node_id",
                principalTable: "node",
                principalColumn: "id");
        }
    }
}
