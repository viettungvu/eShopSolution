using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopSolution.Data.Migrations
{
    public partial class ChangeImageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "Images",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                column: "ConcurrencyStamp",
                value: "c9c82bb5-443d-4b43-8273-db7e28f5b095");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                columns: new[] { "ConcurrencyStamp", "DoB", "PasswordHash" },
                values: new object[] { "5a254331-c2bc-4a40-a3b2-f581f91842ef", new DateTime(2021, 5, 9, 0, 0, 0, 0, DateTimeKind.Local), "AQAAAAEAACcQAAAAEL1qLdc41B0WD9T7jTuMSaIPJDbgZstuuDzJQm5CKDjF0EKUpev7mwFu8F7h0BFu5A==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 9, 18, 35, 22, 930, DateTimeKind.Local).AddTicks(8147));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileSize",
                table: "Images",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                column: "ConcurrencyStamp",
                value: "eae498ea-cf40-4028-b414-06dea39f7913");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("a18be9c0-aa65-4af8-bd17-00bd9344e575"),
                columns: new[] { "ConcurrencyStamp", "DoB", "PasswordHash" },
                values: new object[] { "941d73ba-9d62-448f-8086-8e36ab50287d", new DateTime(2021, 5, 7, 0, 0, 0, 0, DateTimeKind.Local), "AQAAAAEAACcQAAAAEGV05cdKCJiKFj58beyTl3qPdsNEKop/kSncLCn3m+9ukWCoXtS6EP/djpObfDihbQ==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 5, 7, 16, 28, 16, 127, DateTimeKind.Local).AddTicks(8704));
        }
    }
}
