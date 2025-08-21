using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "asp_net_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "asp_net_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "asp_net_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "asp_net_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "asp_net_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_companies", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_company_id",
                table: "asp_net_users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_companies_created_at",
                table: "companies",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_companies_name",
                table: "companies",
                column: "name");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_companies_company_id",
                table: "asp_net_users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_companies_company_id",
                table: "asp_net_users");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_company_id",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "asp_net_users");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "asp_net_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "asp_net_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
