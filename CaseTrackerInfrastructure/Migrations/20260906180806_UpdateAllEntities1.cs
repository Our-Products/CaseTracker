using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseTrackerInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllEntities1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "law_firms",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "law_firms");
        }
    }
}
