using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduHome.App.Migrations
{
    /// <inheritdoc />
    public partial class updatedServiceTableSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socials_Teachers_TeacherId",
                table: "Socials");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Socials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Socials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Socials_ServiceId",
                table: "Socials",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Socials_Services_ServiceId",
                table: "Socials",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Socials_Teachers_TeacherId",
                table: "Socials",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socials_Services_ServiceId",
                table: "Socials");

            migrationBuilder.DropForeignKey(
                name: "FK_Socials_Teachers_TeacherId",
                table: "Socials");

            migrationBuilder.DropIndex(
                name: "IX_Socials_ServiceId",
                table: "Socials");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Socials");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Socials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Socials_Teachers_TeacherId",
                table: "Socials",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
