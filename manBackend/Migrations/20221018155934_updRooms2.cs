using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace manBackend.Migrations
{
    public partial class updRooms2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashId",
                table: "Classrooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashId",
                table: "Classrooms");
        }
    }
}
