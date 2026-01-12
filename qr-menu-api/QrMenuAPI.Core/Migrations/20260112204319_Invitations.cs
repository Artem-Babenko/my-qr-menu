using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrMenuAPI.Core.Migrations
{
    /// <inheritdoc />
    public partial class Invitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    surname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    target_user_id = table.Column<int>(type: "integer", nullable: true),
                    establishment_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_invitations_establishments_establishment_id",
                        column: x => x.establishment_id,
                        principalTable: "establishments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitations_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invitations_users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invitations_establishment_id",
                table: "invitations",
                column: "establishment_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_phone_establishment_id",
                table: "invitations",
                columns: new[] { "phone", "establishment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invitations_role_id",
                table: "invitations",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_target_user_id",
                table: "invitations",
                column: "target_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitations");
        }
    }
}
