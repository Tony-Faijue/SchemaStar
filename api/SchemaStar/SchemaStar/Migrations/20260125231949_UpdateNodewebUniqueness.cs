using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SchemaStar.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNodewebUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: false, defaultValueSql: "(UUID_TO_BIN(UUID(),1))"),
                    username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    pass = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "nodeweb",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: false, defaultValueSql: "(UUID_TO_BIN(UUID(),1))"),
                    node_web_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    last_layout_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    user_id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "nodeweb_ibfk_1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "node",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: false, defaultValueSql: "(UUID_TO_BIN(UUID(),1))"),
                    node_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    node_description = table.Column<string>(type: "text", maxLength: 16383, nullable: true),
                    node_image_url = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    node_audio_url = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    node_image_mime = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    node_audio_mime = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    node_image_size = table.Column<int>(type: "int", nullable: true),
                    node_audio_size = table.Column<int>(type: "int", nullable: true),
                    position_x = table.Column<double>(type: "double", nullable: true),
                    position_y = table.Column<double>(type: "double", nullable: true),
                    width = table.Column<int>(type: "int", nullable: false),
                    height = table.Column<int>(type: "int", nullable: false),
                    state = table.Column<string>(type: "enum('locked','pinned','unlocked')", nullable: true, defaultValueSql: "'unlocked'"),
                    created_by = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    updated_by = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    node_web_id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "node_ibfk_1",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "node_ibfk_2",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "node_ibfk_3",
                        column: x => x.node_web_id,
                        principalTable: "nodeweb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "edge",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: false, defaultValueSql: "(UUID_TO_BIN(UUID(),1))"),
                    edge_type = table.Column<string>(type: "enum('directed','undirected')", nullable: true, defaultValueSql: "'directed'"),
                    from_node_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    to_node_id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "edge_ibfk_1",
                        column: x => x.from_node_id,
                        principalTable: "node",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "edge_ibfk_2",
                        column: x => x.to_node_id,
                        principalTable: "node",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "from_node_id",
                table: "edge",
                column: "from_node_id");

            migrationBuilder.CreateIndex(
                name: "public_id",
                table: "edge",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "to_node_id",
                table: "edge",
                column: "to_node_id");

            migrationBuilder.CreateIndex(
                name: "created_by",
                table: "node",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "node_web_id",
                table: "node",
                column: "node_web_id");

            migrationBuilder.CreateIndex(
                name: "public_id1",
                table: "node",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "updated_by",
                table: "node",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "idx_user_nodename_unique",
                table: "nodeweb",
                columns: new[] { "user_id", "node_web_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "public_id2",
                table: "nodeweb",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_id",
                table: "nodeweb",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "public_id3",
                table: "users",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "edge");

            migrationBuilder.DropTable(
                name: "node");

            migrationBuilder.DropTable(
                name: "nodeweb");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
