using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeManagerApi.Core.Migrations
{
    /// <inheritdoc />
    public partial class EditTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeStamp_Schedules_ScheduleId",
                table: "TimeStamp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeStamp",
                table: "TimeStamp");

            migrationBuilder.RenameTable(
                name: "TimeStamp",
                newName: "TimeStamps");

            migrationBuilder.RenameIndex(
                name: "IX_TimeStamp_ScheduleId",
                table: "TimeStamps",
                newName: "IX_TimeStamps_ScheduleId");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotify",
                table: "TimeStamps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeStamps",
                table: "TimeStamps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeStamps",
                table: "TimeStamps");

            migrationBuilder.DropColumn(
                name: "IsNotify",
                table: "TimeStamps");

            migrationBuilder.RenameTable(
                name: "TimeStamps",
                newName: "TimeStamp");

            migrationBuilder.RenameIndex(
                name: "IX_TimeStamps_ScheduleId",
                table: "TimeStamp",
                newName: "IX_TimeStamp_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeStamp",
                table: "TimeStamp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeStamp_Schedules_ScheduleId",
                table: "TimeStamp",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
