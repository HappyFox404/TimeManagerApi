using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeManagerApi.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixConstraintTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_UserId",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_UserId",
                table: "Schedules",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_UserId",
                table: "Schedules");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_UserId",
                table: "Schedules",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeStamps_Schedules_ScheduleId",
                table: "TimeStamps",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
