using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SystemGuidRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5163), null, "SuperAdmin" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5168), null, "Manager" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5169), null, "President" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5170), null, "Secretary" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5171), null, "Treasurer" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5172), null, "ResidentOwner" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5173), null, "Tenant" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5174), null, "Security" },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new DateTime(2026, 3, 15, 19, 34, 57, 383, DateTimeKind.Utc).AddTicks(5175), null, "Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FlatId", "FullName", "IsActive", "IsOtpVerified", "IsRegistrationCompleted", "PasswordHash", "PrimaryPhone", "ResidentType", "SecondaryPhone", "Status", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 15, 19, 34, 57, 551, DateTimeKind.Utc).AddTicks(6857), "admin@apartment.com", null, "System Administrator", true, true, true, "$2a$11$gRcwrhNB5iMTPHxhe0NZtu8XiDJ8OUqrGmQqwDVHb9n2P3s5Qt97O", "9999999999", null, null, 1, null, null, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt", "Id" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 15, 19, 34, 57, 551, DateTimeKind.Utc).AddTicks(7372), new Guid("1345ef56-90a8-42e9-8ccc-daf281f78432") });
        }
    }
}
