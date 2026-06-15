using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToNodeCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "edge_ibfk_2",
                table: "edge");

            migrationBuilder.AddForeignKey(
                name: "edge_ibfk_2",
                table: "edge",
                column: "to_node_id",
                principalTable: "node",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "edge_ibfk_2",
                table: "edge");

            migrationBuilder.AddForeignKey(
                name: "edge_ibfk_2",
                table: "edge",
                column: "to_node_id",
                principalTable: "node",
                principalColumn: "id");
        }
    }
}
