using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApartmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInvites_Roles_RoleId",
                table: "UserInvites");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryPhone",
                table: "UserInvites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "UserInvites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5972));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5976));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5977));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5978), null, "Resident Owner" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5980), null, "Tenant" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5981), null, "Security" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 12, 10, 24, 5, 350, DateTimeKind.Utc).AddTicks(5982), null, "Maintenance Staff" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 12, 10, 24, 5, 550, DateTimeKind.Utc).AddTicks(1850), "$2a$11$22zWNGkazkVUP88GSaakeenYDGFvoRprNR7C.C84mKYtPeFF1HFd." });

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvites_Roles_RoleId",
                table: "UserInvites",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInvites_Roles_RoleId",
                table: "UserInvites");

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

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryPhone",
                table: "UserInvites",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "UserInvites",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 9, 44, 29, 330, DateTimeKind.Utc).AddTicks(4154));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 9, 44, 29, 330, DateTimeKind.Utc).AddTicks(4158));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 9, 44, 29, 330, DateTimeKind.Utc).AddTicks(4160));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 11, 9, 44, 29, 494, DateTimeKind.Utc).AddTicks(1125), "$2a$11$oCnqr..R4YONyWoLEnak2uy1E0PXL2XWjLXpzPXp1mNzGZW4ltk4O" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvites_Roles_RoleId",
                table: "UserInvites",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
