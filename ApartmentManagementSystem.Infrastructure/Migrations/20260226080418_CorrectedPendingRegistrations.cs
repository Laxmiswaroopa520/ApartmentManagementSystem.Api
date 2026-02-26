using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedPendingRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9651));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9655));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9656));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9657));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9658));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9659));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9660));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 8, 4, 17, 855, DateTimeKind.Utc).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") },
                columns: new[] { "AssignedAt", "Id" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 4, 17, 990, DateTimeKind.Utc).AddTicks(5725), new Guid("f2e0b289-c0f4-41c8-b69a-8e58069fa03a") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 26, 8, 4, 17, 990, DateTimeKind.Utc).AddTicks(5173), "$2a$11$E8gewQHpmi6y7pIXFvGa1e05DJb69IvczbAmp5aon22vdFuNIzB9G" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(572));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(578));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(581));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(584));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(587));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(591));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(594));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(597));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 8, 15, 58, 4, 220, DateTimeKind.Utc).AddTicks(600));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") },
                columns: new[] { "AssignedAt", "Id" },
                values: new object[] { new DateTime(2026, 2, 8, 15, 58, 4, 416, DateTimeKind.Utc).AddTicks(2436), new Guid("7d111e83-bf14-4f64-9df5-9c17269785bb") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 8, 15, 58, 4, 416, DateTimeKind.Utc).AddTicks(1837), "$2a$11$0zkZj0KCItNrtJpp3awnVuA1SzKv5EhexK6o5vm8bKiHqh16BBBFe" });
        }
    }
}
