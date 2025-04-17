﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RO.DevTest.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "aspnet_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    normalized_name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    concurrency_stamp = table.Column<string>(type: "VARCHAR", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    user_name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    normalized_user_name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    normalized_email = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: true),
                    email_confirmed = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    password_hash = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    security_stamp = table.Column<string>(type: "VARCHAR", maxLength: 36, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "VARCHAR", maxLength: 36, nullable: true),
                    phone_number = table.Column<string>(type: "VARCHAR", maxLength: 15, nullable: false),
                    phone_number_confirmed = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    access_failed_count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_role_claim",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "UUID", nullable: false),
                    claim_type = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    claim_value = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_role_claim_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_aspnet_role_claim_aspnet_role_role_id",
                        column: x => x.role_id,
                        principalTable: "aspnet_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_claim",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "UUID", nullable: false),
                    claim_type = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    claim_value = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "FK_aspnet_user_claim_aspnet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_login",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    provider_key = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "UUID", nullable: false),
                    provider_display_name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_login_user_id_login_provider_provider_key", x => new { x.user_id, x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_aspnet_user_login_aspnet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_role",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "UUID", nullable: false),
                    role_id = table.Column<Guid>(type: "UUID", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnet_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_aspnet_user_role_aspnet_role_role_id",
                        column: x => x.role_id,
                        principalTable: "aspnet_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aspnet_user_role_aspnet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_user_token",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "UUID", nullable: false),
                    login_provider = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    value = table.Column<string>(type: "VARCHAR", maxLength: 755, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_user_token_user_id_login_provider_name", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_aspnet_user_token_aspnet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    description = table.Column<string>(type: "VARCHAR(455)", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "0"),
                    SallerId = table.Column<Guid>(type: "UUID", nullable: false),
                    created_on = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modified_on = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_saller",
                        column: x => x.SallerId,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "UUID", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    saller_id = table.Column<Guid>(type: "UUID", nullable: false),
                    product_id = table.Column<Guid>(type: "UUID", nullable: false),
                    buyer_id = table.Column<Guid>(type: "UUID", nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_on = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    modified_on = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_buyer",
                        column: x => x.buyer_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sales_product",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sales_saller",
                        column: x => x.saller_id,
                        principalTable: "aspnet_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_role_normalized_name",
                table: "aspnet_role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aspnet_role_claim_role_id",
                table: "aspnet_role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "aspnet_user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_email",
                table: "aspnet_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_user_normalized_user_name",
                table: "aspnet_user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aspnet_user_claim_user_id",
                table: "aspnet_user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_aspnet_user_role_role_id",
                table: "aspnet_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_SallerId",
                table: "product",
                column: "SallerId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_buyer_id",
                table: "sale",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_product_id",
                table: "sale",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_saller_id",
                table: "sale",
                column: "saller_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnet_role_claim");

            migrationBuilder.DropTable(
                name: "aspnet_user_claim");

            migrationBuilder.DropTable(
                name: "aspnet_user_login");

            migrationBuilder.DropTable(
                name: "aspnet_user_role");

            migrationBuilder.DropTable(
                name: "aspnet_user_token");

            migrationBuilder.DropTable(
                name: "sale");

            migrationBuilder.DropTable(
                name: "aspnet_role");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "aspnet_user");
        }
    }
}
