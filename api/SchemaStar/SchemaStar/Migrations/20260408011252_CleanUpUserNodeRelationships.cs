using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpUserNodeRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "node_ibfk_1",
                table: "node");

            migrationBuilder.DropForeignKey(
                name: "node_ibfk_2",
                table: "node");

            migrationBuilder.DropIndex(
                name: "created_by",
                table: "node");

            migrationBuilder.DropIndex(
                name: "updated_by",
                table: "node");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "node");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "node");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "created_by",
                table: "node",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "updated_by",
                table: "node",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "created_by",
                table: "node",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "updated_by",
                table: "node",
                column: "updated_by");

            migrationBuilder.AddForeignKey(
                name: "node_ibfk_1",
                table: "node",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "node_ibfk_2",
                table: "node",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
