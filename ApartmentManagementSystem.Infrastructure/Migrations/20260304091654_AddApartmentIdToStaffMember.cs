using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApartmentIdToStaffMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId",
                table: "StaffMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1795));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1798));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1799));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1800));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1801));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1802));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1803));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1803));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 16, 53, 992, DateTimeKind.Utc).AddTicks(1804));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") },
                columns: new[] { "AssignedAt", "Id" },
                values: new object[] { new DateTime(2026, 3, 4, 9, 16, 54, 126, DateTimeKind.Utc).AddTicks(1725), new Guid("57032858-ead8-46ec-8704-f91bcf76f490") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 4, 9, 16, 54, 126, DateTimeKind.Utc).AddTicks(1148), "$2a$11$6ra1QMVy7eU0mT5cgeFNc..aXqGjTvdBH7b6s8eYKk5A3xyPF9nsm" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_ApartmentId",
                table: "StaffMembers",
                column: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_Apartments_ApartmentId",
                table: "StaffMembers",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_Apartments_ApartmentId",
                table: "StaffMembers");

            migrationBuilder.DropIndex(
                name: "IX_StaffMembers_ApartmentId",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "StaffMembers");

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
    }
}
