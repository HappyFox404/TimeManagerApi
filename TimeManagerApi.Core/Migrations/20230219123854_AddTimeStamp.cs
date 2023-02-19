using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeManagerApi.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_UserId",
                table: "Schedules");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_UserId",
                table: "Schedules",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "TimeStamp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeStamp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeStamp_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeStamp_ScheduleId",
                table: "TimeStamp",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeStamp");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_UserId",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_UserId",
                table: "Schedules",
                column: "UserId");
        }
    }
}
