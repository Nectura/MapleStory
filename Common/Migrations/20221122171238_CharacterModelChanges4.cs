using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class CharacterModelChanges4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "Characters");

            migrationBuilder.AddColumn<ushort>(
                name: "SubJob",
                table: "Characters",
                type: "smallint unsigned",
                nullable: false,
                defaultValue: (ushort)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubJob",
                table: "Characters");

            migrationBuilder.AddColumn<uint>(
                name: "JobCategory",
                table: "Characters",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
