using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CommunityMembersMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JoinedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Apartments",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(5953));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(6180));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(6186));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(6189));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(6225));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(815));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(819));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000009"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(827));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000010"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(835));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000011"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(843));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000012"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(847));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000013"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(850));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000014"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(853));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000015"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(857));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000016"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(860));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000017"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(863));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000018"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(867));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000019"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(871));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000020"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(875));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000021"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(879));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000022"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(882));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000023"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(886));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000024"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(889));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000025"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1021));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000026"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1024));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000027"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1028));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000028"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1031));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000029"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1034));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000030"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1038));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000031"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1042));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000032"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1045));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000033"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1048));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000034"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1055));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000035"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1058));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000036"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1061));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000037"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1064));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000038"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1067));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000039"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1070));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000040"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1073));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000041"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1078));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000042"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1081));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000043"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1084));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000044"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1087));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000045"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1117));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000046"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1121));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000047"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1124));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000048"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1127));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000049"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1131));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000050"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1134));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000051"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1139));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000052"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1142));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000053"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1145));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000054"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1147));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000055"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1150));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000056"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1153));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000057"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1156));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000058"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1159));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000059"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1162));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000060"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1165));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000061"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1169));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000062"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1172));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000063"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1174));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000064"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1177));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000065"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1180));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000066"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1218));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000067"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000068"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1224));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000069"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1227));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000070"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1231));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000071"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1235));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000072"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1238));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000073"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1240));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000074"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1243));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000075"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1246));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000076"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1249));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000077"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1252));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000078"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1254));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000079"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1257));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000080"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1260));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000081"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1264));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000082"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1267));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000083"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1270));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000084"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1273));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000085"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1275));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000086"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1278));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000087"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1310));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000088"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1313));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000089"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1316));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000090"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1320));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000091"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1325));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000092"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1328));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000093"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1331));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000094"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1334));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000095"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1337));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000096"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1340));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000097"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1343));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000098"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1346));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000099"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1349));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000100"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1352));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000101"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1356));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000102"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1360));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000103"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1363));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000104"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1365));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000105"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1368));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000106"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1371));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000107"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1374));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000108"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1377));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000109"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1404));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000110"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1407));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000111"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1411));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000112"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1414));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000113"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1417));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000114"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1420));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000115"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1423));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000116"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1426));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000117"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1429));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000118"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1432));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000119"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1435));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000120"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1438));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000121"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1442));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000122"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1444));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000123"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1447));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000124"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1450));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000125"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1453));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000126"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1456));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000127"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1459));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000128"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1462));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000129"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1465));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000130"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1491));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000131"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1496));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000132"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1499));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000133"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1501));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000134"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1504));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000135"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1507));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000136"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1510));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000137"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1512));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000138"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1516));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000139"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1519));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000140"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1522));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000141"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1526));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000142"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1529));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000143"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1532));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000144"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1535));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000145"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1537));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000146"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1540));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000147"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1595));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000148"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1598));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000149"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1601));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000150"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1604));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000151"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1608));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000152"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1612));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000153"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1615));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000154"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1617));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000155"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1620));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000156"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1623));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000157"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1626));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000158"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1629));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000159"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1631));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000160"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1634));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000161"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1638));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000162"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1641));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000163"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1644));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000164"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1647));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000165"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1650));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000166"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1652));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000167"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1655));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000168"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1658));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000169"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1696));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000170"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1700));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000171"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1705));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000172"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1708));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000173"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1711));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000174"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1714));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000175"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1717));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000176"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1720));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000177"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1723));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000178"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1725));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000179"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1728));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000180"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1731));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000181"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1736));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000182"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1739));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000183"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1741));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000184"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1744));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000185"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1747));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000186"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1750));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000187"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1753));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000188"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1756));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000189"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1759));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000190"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1762));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000191"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1790));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000192"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1793));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000193"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000194"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1799));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000195"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1802));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000196"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1805));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000197"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1807));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000198"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1810));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000199"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1813));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000200"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 260, DateTimeKind.Utc).AddTicks(1816));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6626));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6629));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6631));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6631));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6632));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6633));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6634));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 22, 9, 27, 38, 101, DateTimeKind.Utc).AddTicks(6635));

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedBy" },
                values: new object[] { new DateTime(2026, 1, 22, 9, 27, 38, 254, DateTimeKind.Utc).AddTicks(5157), "$2a$11$465UdNMkZsm6I.2vEoztnu6N8SYyaUl.saSQo/RL9gbgBHNmJwgA6", null });

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_Phone",
                table: "StaffMembers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_StaffType",
                table: "StaffMembers",
                column: "StaffType");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_UserId",
                table: "StaffMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Apartments",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(7109));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9372));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9396));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9410));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9429));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9447));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9453));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9459));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000009"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9465));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000010"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9479));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000011"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9488));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000012"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9494));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000013"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9500));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000014"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9514));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000015"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000016"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9535));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000017"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9541));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000018"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9547));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000019"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9553));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000020"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9559));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000021"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9568));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000022"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9573));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000023"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9579));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000024"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9585));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000025"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9591));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000026"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9597));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000027"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9603));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000028"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9608));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000029"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000030"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9623));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000031"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9632));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000032"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9638));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000033"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9644));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000034"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9658));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000035"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000036"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9670));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000037"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9676));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000038"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9682));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000039"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9687));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000040"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9693));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000041"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9702));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000042"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9708));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000043"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9714));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000044"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9719));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000045"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9726));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000046"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9732));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000047"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9737));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000048"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9743));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000049"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9749));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000050"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9755));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000051"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9763));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000052"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9769));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000053"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000054"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9780));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000055"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9792));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000056"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9798));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000057"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9804));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000058"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9810));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000059"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9816));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000060"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9822));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000061"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9831));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000062"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9837));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000063"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9843));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000064"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9849));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000065"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9855));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000066"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9862));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000067"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9868));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000068"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9874));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000069"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9880));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000070"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9886));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000071"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9894));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000072"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000073"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9913));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000074"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9919));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000075"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9925));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000076"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9931));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000077"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9937));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000078"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9943));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000079"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9949));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000080"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9955));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000081"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9963));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000082"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9969));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000083"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9975));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000084"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9981));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000085"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9987));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000086"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9993));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000087"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(9999));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000088"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(5));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000089"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(11));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000090"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(17));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000091"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(25));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000092"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(31));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000093"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(37));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000094"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(49));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000095"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(55));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000096"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(61));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000097"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(67));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000098"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(73));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000099"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(79));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000100"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(85));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000101"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(93));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000102"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(100));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000103"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(105));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000104"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(111));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000105"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(117));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000106"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(123));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000107"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(129));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000108"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(135));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000109"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(141));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000110"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(148));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000111"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(156));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000112"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(162));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000113"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(168));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000114"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000115"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(187));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000116"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(194));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000117"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(199));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000118"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(206));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000119"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(211));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000120"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(217));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000121"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(226));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000122"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(231));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000123"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(237));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000124"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(243));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000125"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000126"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(255));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000127"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(262));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000128"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(268));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000129"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(274));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000130"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(281));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000131"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(296));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000132"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(302));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000133"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(308));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000134"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(314));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000135"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(320));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000136"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(326));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000137"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(332));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000138"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(338));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000139"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(344));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000140"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(350));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000141"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(358));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000142"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(364));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000143"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(370));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000144"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(376));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000145"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(382));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000146"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(388));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000147"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(394));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000148"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(399));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000149"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(405));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000150"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(411));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000151"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(427));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000152"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(433));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000153"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(439));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000154"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(445));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000155"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(451));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000156"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(456));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000157"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(462));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000158"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(468));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000159"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(474));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000160"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(480));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000161"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(488));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000162"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(495));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000163"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(500));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000164"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(506));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000165"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(512));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000166"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(518));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000167"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(525));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000168"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(532));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000169"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(537));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000170"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(544));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000171"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(552));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000172"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(564));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000173"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(570));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000174"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(575));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000175"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(581));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000176"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(587));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000177"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(593));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000178"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(598));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000179"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(604));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000180"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(610));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000181"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(618));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000182"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(624));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000183"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(630));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000184"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(636));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000185"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(642));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000186"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(648));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000187"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(654));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000188"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(660));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000189"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(666));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000190"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(672));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000191"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(681));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000192"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(687));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000193"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(698));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000194"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(704));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000195"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(710));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000196"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(716));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000197"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(722));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000198"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(728));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000199"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(735));

            migrationBuilder.UpdateData(
                table: "Flats",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000200"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 547, DateTimeKind.Utc).AddTicks(741));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3002));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3005));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3006));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3008));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3009));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3010));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3012));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 21, 2, 6, 13, 374, DateTimeKind.Utc).AddTicks(3013));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2026, 1, 21, 2, 6, 13, 546, DateTimeKind.Utc).AddTicks(6556), "$2a$11$LjIhNdhWlkrnqVnj5hf5F.vTZY37JhjtVg2APQOrD2lxSuhOsajcm", new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
