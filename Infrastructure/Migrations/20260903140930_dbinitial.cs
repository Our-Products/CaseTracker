using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dbinitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lawyers",
                columns: table => new
                {
                    lawyer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    mobile_number = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lawyers", x => x.lawyer_id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_lawyers_status",
                table: "lawyers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_lawyers_email",
                table: "lawyers",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_lawyers_mobile_number",
                table: "lawyers",
                column: "mobile_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lawyers");
        }
    }
}
