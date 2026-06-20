using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class CompositeIndexEdgeFromTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_edge_from_to",
                table: "edge",
                columns: new[] { "from_node_id", "to_node_id" });

            migrationBuilder.DropIndex(
                name: "from_node_id",
                table: "edge");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "from_node_id",
                table: "edge",
                column: "from_node_id");

            migrationBuilder.DropIndex(
                name: "ix_edge_from_to",
                table: "edge");
        }
    }
}
