using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CursoLog.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLogoUrlFromTechnology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Technologies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Technologies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
