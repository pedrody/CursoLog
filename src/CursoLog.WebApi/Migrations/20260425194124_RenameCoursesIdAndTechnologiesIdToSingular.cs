using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CursoLog.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoursesIdAndTechnologiesIdToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTechnologies_Courses_CourseId",
                table: "CourseTechnologies");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTechnologies_Technologies_TechnologyId",
                table: "CourseTechnologies");

            migrationBuilder.RenameColumn(
                name: "TechnologyId",
                table: "CourseTechnologies",
                newName: "TechnologiesId");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseTechnologies",
                newName: "CoursesId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTechnologies_TechnologyId",
                table: "CourseTechnologies",
                newName: "IX_CourseTechnologies_TechnologiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTechnologies_Courses_CoursesId",
                table: "CourseTechnologies",
                column: "CoursesId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTechnologies_Technologies_TechnologiesId",
                table: "CourseTechnologies",
                column: "TechnologiesId",
                principalTable: "Technologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTechnologies_Courses_CoursesId",
                table: "CourseTechnologies");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTechnologies_Technologies_TechnologiesId",
                table: "CourseTechnologies");

            migrationBuilder.RenameColumn(
                name: "TechnologiesId",
                table: "CourseTechnologies",
                newName: "TechnologyId");

            migrationBuilder.RenameColumn(
                name: "CoursesId",
                table: "CourseTechnologies",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTechnologies_TechnologiesId",
                table: "CourseTechnologies",
                newName: "IX_CourseTechnologies_TechnologyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTechnologies_Courses_CourseId",
                table: "CourseTechnologies",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTechnologies_Technologies_TechnologyId",
                table: "CourseTechnologies",
                column: "TechnologyId",
                principalTable: "Technologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
