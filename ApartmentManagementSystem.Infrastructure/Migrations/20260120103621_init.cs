using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TotalFlats = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Floors_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentType = table.Column<int>(type: "int", nullable: false),
                    InviteStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInvites_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FloorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flats_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flats_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResidentType = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsOtpVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsRegistrationCompleted = table.Column<bool>(type: "bit", nullable: false),
                    FlatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Flats_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFlatMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlatMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlatMappings_Flats_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFlatMappings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOtps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOtps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Apartments",
                columns: new[] { "Id", "Address", "CreatedAt", "Name", "TotalFlats", "UpdatedAt" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000001"), "123 Main Street, Chennai", new DateTime(2026, 1, 20, 10, 36, 20, 967, DateTimeKind.Utc).AddTicks(621), "Green Valley Apartments", 200, null });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6272), null, "SuperAdmin" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6280), null, "President" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6282), null, "Secretary" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6283), null, "Treasurer" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6284), null, "Resident Owner" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6285), null, "Tenant" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6286), null, "Security" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 20, 10, 36, 20, 810, DateTimeKind.Utc).AddTicks(6287), null, "Maintenance Staff" }
                });

            migrationBuilder.InsertData(
                table: "Floors",
                columns: new[] { "Id", "ApartmentId", "FloorNumber", "Name" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), 1, "Floor 1" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000001"), 2, "Floor 2" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000001"), 3, "Floor 3" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new Guid("30000000-0000-0000-0000-000000000001"), 4, "Floor 4" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new Guid("30000000-0000-0000-0000-000000000001"), 5, "Floor 5" },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new Guid("30000000-0000-0000-0000-000000000001"), 6, "Floor 6" },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new Guid("30000000-0000-0000-0000-000000000001"), 7, "Floor 7" },
                    { new Guid("40000000-0000-0000-0000-000000000008"), new Guid("30000000-0000-0000-0000-000000000001"), 8, "Floor 8" },
                    { new Guid("40000000-0000-0000-0000-000000000009"), new Guid("30000000-0000-0000-0000-000000000001"), 9, "Floor 9" },
                    { new Guid("40000000-0000-0000-0000-000000000010"), new Guid("30000000-0000-0000-0000-000000000001"), 10, "Floor 10" },
                    { new Guid("40000000-0000-0000-0000-000000000011"), new Guid("30000000-0000-0000-0000-000000000001"), 11, "Floor 11" },
                    { new Guid("40000000-0000-0000-0000-000000000012"), new Guid("30000000-0000-0000-0000-000000000001"), 12, "Floor 12" },
                    { new Guid("40000000-0000-0000-0000-000000000013"), new Guid("30000000-0000-0000-0000-000000000001"), 13, "Floor 13" },
                    { new Guid("40000000-0000-0000-0000-000000000014"), new Guid("30000000-0000-0000-0000-000000000001"), 14, "Floor 14" },
                    { new Guid("40000000-0000-0000-0000-000000000015"), new Guid("30000000-0000-0000-0000-000000000001"), 15, "Floor 15" },
                    { new Guid("40000000-0000-0000-0000-000000000016"), new Guid("30000000-0000-0000-0000-000000000001"), 16, "Floor 16" },
                    { new Guid("40000000-0000-0000-0000-000000000017"), new Guid("30000000-0000-0000-0000-000000000001"), 17, "Floor 17" },
                    { new Guid("40000000-0000-0000-0000-000000000018"), new Guid("30000000-0000-0000-0000-000000000001"), 18, "Floor 18" },
                    { new Guid("40000000-0000-0000-0000-000000000019"), new Guid("30000000-0000-0000-0000-000000000001"), 19, "Floor 19" },
                    { new Guid("40000000-0000-0000-0000-000000000020"), new Guid("30000000-0000-0000-0000-000000000001"), 20, "Floor 20" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FlatId", "FullName", "IsActive", "IsOtpVerified", "IsRegistrationCompleted", "PasswordHash", "PrimaryPhone", "ResidentType", "RoleId", "SecondaryPhone", "Status", "UpdatedAt", "Username" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 966, DateTimeKind.Utc).AddTicks(9413), "admin@apartment.com", null, "System Administrator", true, true, true, "$2a$11$jOS76NzTMy3xOX/vdQ4OHuJpf87t3Z8iSNDgwA5Pzt.liTEf1VvH.", "9999999999", null, new Guid("10000000-0000-0000-0000-000000000001"), null, 1, null, "admin" });

            migrationBuilder.InsertData(
                table: "Flats",
                columns: new[] { "Id", "ApartmentId", "CreatedAt", "FlatNumber", "FloorId", "IsActive", "IsOccupied", "Name", "OwnerUserId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 967, DateTimeKind.Utc).AddTicks(1346), "101", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 101", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 967, DateTimeKind.Utc).AddTicks(1359), "102", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 102", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 967, DateTimeKind.Utc).AddTicks(1366), "103", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 103", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000004"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 967, DateTimeKind.Utc).AddTicks(1483), "104", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 104", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000005"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9433), "105", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 105", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000006"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9463), "106", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 106", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000007"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9470), "107", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 107", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000008"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9475), "108", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 108", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000009"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9480), "109", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 109", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000010"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9490), "110", new Guid("40000000-0000-0000-0000-000000000001"), true, false, "Flat 110", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000011"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9500), "201", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 201", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000012"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9505), "202", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 202", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000013"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9510), "203", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 203", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000014"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9515), "204", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 204", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000015"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9521), "205", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 205", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000016"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9526), "206", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 206", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000017"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9531), "207", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 207", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000018"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9540), "208", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 208", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000019"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9545), "209", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 209", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000020"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9550), "210", new Guid("40000000-0000-0000-0000-000000000002"), true, false, "Flat 210", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000021"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9559), "301", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 301", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000022"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9566), "302", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 302", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000023"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9572), "303", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 303", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000024"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9701), "304", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 304", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000025"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9709), "305", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 305", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000026"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9715), "306", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 306", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000027"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9720), "307", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 307", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000028"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9727), "308", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 308", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000029"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9733), "309", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 309", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000030"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9739), "310", new Guid("40000000-0000-0000-0000-000000000003"), true, false, "Flat 310", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000031"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9747), "401", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 401", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000032"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9753), "402", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 402", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000033"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9759), "403", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 403", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000034"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9767), "404", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 404", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000035"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9771), "405", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 405", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000036"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9776), "406", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 406", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000037"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9781), "407", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 407", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000038"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9785), "408", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 408", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000039"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9790), "409", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 409", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000040"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9795), "410", new Guid("40000000-0000-0000-0000-000000000004"), true, false, "Flat 410", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000041"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9803), "501", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 501", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000042"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9808), "502", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 502", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000043"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9814), "503", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 503", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000044"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9820), "504", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 504", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000045"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9886), "505", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 505", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000046"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9892), "506", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 506", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000047"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9898), "507", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 507", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000048"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9904), "508", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 508", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000049"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9909), "509", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 509", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000050"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9915), "510", new Guid("40000000-0000-0000-0000-000000000005"), true, false, "Flat 510", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000051"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9922), "601", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 601", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000052"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9926), "602", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 602", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000053"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9931), "603", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 603", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000054"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9936), "604", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 604", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000055"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9940), "605", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 605", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000056"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9945), "606", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 606", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000057"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9949), "607", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 607", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000058"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9954), "608", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 608", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000059"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9960), "609", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 609", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000060"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9965), "610", new Guid("40000000-0000-0000-0000-000000000006"), true, false, "Flat 610", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000061"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9973), "701", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 701", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000062"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9978), "702", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 702", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000063"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9983), "703", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 703", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000064"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9988), "704", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 704", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000065"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 968, DateTimeKind.Utc).AddTicks(9992), "705", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 705", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000066"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(41), "706", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 706", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000067"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(46), "707", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 707", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000068"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(50), "708", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 708", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000069"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(55), "709", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 709", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000070"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(60), "710", new Guid("40000000-0000-0000-0000-000000000007"), true, false, "Flat 710", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000071"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(66), "801", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 801", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000072"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(71), "802", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 802", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000073"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(77), "803", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 803", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000074"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(82), "804", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 804", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000075"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(87), "805", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 805", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000076"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(92), "806", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 806", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000077"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(97), "807", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 807", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000078"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(103), "808", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 808", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000079"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(108), "809", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 809", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000080"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(113), "810", new Guid("40000000-0000-0000-0000-000000000008"), true, false, "Flat 810", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000081"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(120), "901", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 901", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000082"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(125), "902", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 902", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000083"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(130), "903", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 903", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000084"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(135), "904", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 904", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000085"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(140), "905", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 905", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000086"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(189), "906", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 906", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000087"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(195), "907", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 907", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000088"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(200), "908", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 908", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000089"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(205), "909", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 909", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000090"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(210), "910", new Guid("40000000-0000-0000-0000-000000000009"), true, false, "Flat 910", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000091"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(218), "1001", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1001", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000092"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(224), "1002", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1002", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000093"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(231), "1003", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1003", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000094"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(237), "1004", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1004", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000095"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(244), "1005", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1005", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000096"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(250), "1006", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1006", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000097"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(255), "1007", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1007", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000098"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(262), "1008", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1008", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000099"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(268), "1009", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1009", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000100"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(274), "1010", new Guid("40000000-0000-0000-0000-000000000010"), true, false, "Flat 1010", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000101"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(281), "1101", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1101", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000102"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(285), "1102", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1102", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000103"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(291), "1103", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1103", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000104"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(297), "1104", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1104", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000105"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(301), "1105", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1105", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000106"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(307), "1106", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1106", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000107"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(312), "1107", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1107", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000108"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(319), "1108", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1108", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000109"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(362), "1109", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1109", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000110"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(368), "1110", new Guid("40000000-0000-0000-0000-000000000011"), true, false, "Flat 1110", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000111"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(377), "1201", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1201", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000112"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(382), "1202", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1202", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000113"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(388), "1203", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1203", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000114"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(394), "1204", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1204", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000115"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(399), "1205", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1205", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000116"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(405), "1206", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1206", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000117"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(411), "1207", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1207", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000118"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(417), "1208", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1208", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000119"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(423), "1209", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1209", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000120"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(428), "1210", new Guid("40000000-0000-0000-0000-000000000012"), true, false, "Flat 1210", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000121"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(436), "1301", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1301", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000122"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(444), "1302", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1302", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000123"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(450), "1303", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1303", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000124"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(457), "1304", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1304", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000125"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(463), "1305", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1305", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000126"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(469), "1306", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1306", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000127"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(476), "1307", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1307", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000128"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(481), "1308", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1308", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000129"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(487), "1309", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1309", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000130"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(542), "1310", new Guid("40000000-0000-0000-0000-000000000013"), true, false, "Flat 1310", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000131"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(553), "1401", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1401", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000132"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(560), "1402", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1402", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000133"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(567), "1403", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1403", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000134"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(574), "1404", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1404", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000135"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(581), "1405", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1405", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000136"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(587), "1406", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1406", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000137"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(592), "1407", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1407", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000138"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(599), "1408", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1408", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000139"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(605), "1409", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1409", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000140"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(610), "1410", new Guid("40000000-0000-0000-0000-000000000014"), true, false, "Flat 1410", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000141"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(618), "1501", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1501", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000142"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(624), "1502", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1502", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000143"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(630), "1503", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1503", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000144"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(635), "1504", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1504", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000145"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(640), "1505", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1505", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000146"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(645), "1506", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1506", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000147"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(692), "1507", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1507", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000148"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(699), "1508", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1508", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000149"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(706), "1509", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1509", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000150"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(712), "1510", new Guid("40000000-0000-0000-0000-000000000015"), true, false, "Flat 1510", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000151"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(720), "1601", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1601", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000152"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(727), "1602", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1602", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000153"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(732), "1603", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1603", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000154"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(737), "1604", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1604", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000155"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(743), "1605", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1605", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000156"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(748), "1606", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1606", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000157"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(753), "1607", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1607", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000158"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(758), "1608", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1608", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000159"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(763), "1609", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1609", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000160"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(769), "1610", new Guid("40000000-0000-0000-0000-000000000016"), true, false, "Flat 1610", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000161"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(778), "1701", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1701", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000162"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(784), "1702", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1702", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000163"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(789), "1703", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1703", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000164"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(794), "1704", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1704", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000165"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(799), "1705", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1705", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000166"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(804), "1706", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1706", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000167"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(809), "1707", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1707", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000168"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(815), "1708", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1708", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000169"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(819), "1709", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1709", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000170"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(854), "1710", new Guid("40000000-0000-0000-0000-000000000017"), true, false, "Flat 1710", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000171"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(863), "1801", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1801", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000172"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(868), "1802", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1802", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000173"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(873), "1803", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1803", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000174"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(878), "1804", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1804", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000175"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(885), "1805", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1805", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000176"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(891), "1806", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1806", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000177"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(897), "1807", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1807", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000178"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(903), "1808", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1808", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000179"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(909), "1809", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1809", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000180"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(915), "1810", new Guid("40000000-0000-0000-0000-000000000018"), true, false, "Flat 1810", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000181"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(923), "1901", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1901", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000182"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(928), "1902", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1902", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000183"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(933), "1903", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1903", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000184"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(939), "1904", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1904", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000185"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(945), "1905", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1905", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000186"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(951), "1906", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1906", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000187"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(957), "1907", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1907", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000188"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(963), "1908", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1908", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000189"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(968), "1909", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1909", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000190"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(974), "1910", new Guid("40000000-0000-0000-0000-000000000019"), true, false, "Flat 1910", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000191"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1043), "2001", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2001", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000192"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1048), "2002", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2002", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000193"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1055), "2003", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2003", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000194"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1061), "2004", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2004", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000195"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1066), "2005", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2005", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000196"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1071), "2006", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2006", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000197"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1076), "2007", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2007", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000198"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1080), "2008", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2008", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000199"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1086), "2009", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2009", null, null },
                    { new Guid("50000000-0000-0000-0000-000000000200"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 10, 36, 20, 969, DateTimeKind.Utc).AddTicks(1091), "2010", new Guid("40000000-0000-0000-0000-000000000020"), true, false, "Flat 2010", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flats_ApartmentId_FlatNumber",
                table: "Flats",
                columns: new[] { "ApartmentId", "FlatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flats_FloorId",
                table: "Flats",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_OwnerUserId",
                table: "Flats",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Floors_ApartmentId",
                table: "Floors",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlatMappings_FlatId",
                table: "UserFlatMappings",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlatMappings_UserId_FlatId_IsActive",
                table: "UserFlatMappings",
                columns: new[] { "UserId", "FlatId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserInvites_RoleId",
                table: "UserInvites",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOtps_UserId",
                table: "UserOtps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FlatId",
                table: "Users",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flats_Users_OwnerUserId",
                table: "Flats",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flats_Apartments_ApartmentId",
                table: "Flats");

            migrationBuilder.DropForeignKey(
                name: "FK_Floors_Apartments_ApartmentId",
                table: "Floors");

            migrationBuilder.DropForeignKey(
                name: "FK_Flats_Floors_FloorId",
                table: "Flats");

            migrationBuilder.DropForeignKey(
                name: "FK_Flats_Users_OwnerUserId",
                table: "Flats");

            migrationBuilder.DropTable(
                name: "UserFlatMappings");

            migrationBuilder.DropTable(
                name: "UserInvites");

            migrationBuilder.DropTable(
                name: "UserOtps");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Flats");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
