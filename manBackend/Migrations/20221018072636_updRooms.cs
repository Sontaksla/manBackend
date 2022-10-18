using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace manBackend.Migrations
{
    public partial class updRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classroom_Users_TeacherId",
                table: "Classroom");

            migrationBuilder.DropForeignKey(
                name: "FK_Classroom_Users_UserId",
                table: "Classroom");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Classroom_ClassroomId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classroom",
                table: "Classroom");

            migrationBuilder.RenameTable(
                name: "Classroom",
                newName: "Classrooms");

            migrationBuilder.RenameIndex(
                name: "IX_Classroom_UserId",
                table: "Classrooms",
                newName: "IX_Classrooms_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Classroom_TeacherId",
                table: "Classrooms",
                newName: "IX_Classrooms_TeacherId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Classrooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classrooms",
                table: "Classrooms",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassroomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercise_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_ClassroomId",
                table: "Exercise",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classrooms_Users_TeacherId",
                table: "Classrooms",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classrooms_Users_UserId",
                table: "Classrooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Classrooms_ClassroomId",
                table: "Users",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classrooms_Users_TeacherId",
                table: "Classrooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Classrooms_Users_UserId",
                table: "Classrooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Classrooms_ClassroomId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classrooms",
                table: "Classrooms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Classrooms");

            migrationBuilder.RenameTable(
                name: "Classrooms",
                newName: "Classroom");

            migrationBuilder.RenameIndex(
                name: "IX_Classrooms_UserId",
                table: "Classroom",
                newName: "IX_Classroom_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Classrooms_TeacherId",
                table: "Classroom",
                newName: "IX_Classroom_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classroom",
                table: "Classroom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classroom_Users_TeacherId",
                table: "Classroom",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classroom_Users_UserId",
                table: "Classroom",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Classroom_ClassroomId",
                table: "Users",
                column: "ClassroomId",
                principalTable: "Classroom",
                principalColumn: "Id");
        }
    }
}
