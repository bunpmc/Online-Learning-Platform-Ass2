using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearningPlatformAss2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastViewedLessonIdToEnrollment4 : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastViewedLessonId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastViewedLessonId",
                table: "Enrollments");
        }


    }
}
