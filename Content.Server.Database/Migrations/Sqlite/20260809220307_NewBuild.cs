using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class NewBuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "organ_markings",
                table: "profile");

            migrationBuilder.AddColumn<string>(
                name: "bark_voice",
                table: "profile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "last_rolled_antag",
                table: "player",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "server_currency",
                table: "player",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "polls",
                columns: table => new
                {
                    polls_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    start_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    end_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_by_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    active = table.Column<bool>(type: "INTEGER", nullable: false),
                    allow_multiple_choices = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_polls", x => x.polls_id);
                    table.ForeignKey(
                        name: "FK_polls_player_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "rmc_discord_accounts",
                columns: table => new
                {
                    rmc_discord_accounts_id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_discord_accounts", x => x.rmc_discord_accounts_id);
                });

            migrationBuilder.CreateTable(
                name: "rmc_linking_codes",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    code = table.Column<Guid>(type: "TEXT", nullable: false),
                    creation_time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_linking_codes", x => x.player_id);
                    table.ForeignKey(
                        name: "FK_rmc_linking_codes_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_patron_tiers",
                columns: table => new
                {
                    rmc_patron_tiers_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    show_on_credits = table.Column<bool>(type: "INTEGER", nullable: false),
                    ghost_color = table.Column<bool>(type: "INTEGER", nullable: false),
                    ghost_cosmetics = table.Column<bool>(type: "INTEGER", nullable: false),
                    ghost_particles = table.Column<bool>(type: "INTEGER", nullable: false),
                    lobby_message = table.Column<bool>(type: "INTEGER", nullable: false),
                    round_end_shoutout = table.Column<bool>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    icon = table.Column<string>(type: "TEXT", nullable: true),
                    discord_role = table.Column<ulong>(type: "INTEGER", nullable: false),
                    priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_patron_tiers", x => x.rmc_patron_tiers_id);
                });

            migrationBuilder.CreateTable(
                name: "poll_options",
                columns: table => new
                {
                    poll_options_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    poll_id = table.Column<int>(type: "INTEGER", nullable: false),
                    option_text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    display_order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_options", x => x.poll_options_id);
                    table.ForeignKey(
                        name: "FK_poll_options_polls_poll_id",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "polls_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "poll_seen",
                columns: table => new
                {
                    poll_seen_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    poll_id = table.Column<int>(type: "INTEGER", nullable: false),
                    player_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    seen_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_seen", x => x.poll_seen_id);
                    table.ForeignKey(
                        name: "FK_poll_seen_player_player_user_id",
                        column: x => x.player_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_poll_seen_polls_poll_id",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "polls_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_linked_accounts",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    discord_id = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_linked_accounts", x => x.player_id);
                    table.ForeignKey(
                        name: "FK_rmc_linked_accounts_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rmc_linked_accounts_rmc_discord_accounts_discord_id",
                        column: x => x.discord_id,
                        principalTable: "rmc_discord_accounts",
                        principalColumn: "rmc_discord_accounts_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_linked_accounts_logs",
                columns: table => new
                {
                    rmc_linked_accounts_logs_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    discord_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_linked_accounts_logs", x => x.rmc_linked_accounts_logs_id);
                    table.ForeignKey(
                        name: "FK_rmc_linked_accounts_logs_player_player_id1",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rmc_linked_accounts_logs_rmc_discord_accounts_discord_id",
                        column: x => x.discord_id,
                        principalTable: "rmc_discord_accounts",
                        principalColumn: "rmc_discord_accounts_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_patrons",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    tier_id = table.Column<int>(type: "INTEGER", nullable: false),
                    ghost_color = table.Column<int>(type: "INTEGER", nullable: true),
                    ghost_particles = table.Column<string>(type: "TEXT", nullable: true),
                    ghost_hat = table.Column<string>(type: "TEXT", nullable: true),
                    ghost_mask = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_patrons", x => x.player_id);
                    table.ForeignKey(
                        name: "FK_rmc_patrons_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rmc_patrons_rmc_patron_tiers_tier_id",
                        column: x => x.tier_id,
                        principalTable: "rmc_patron_tiers",
                        principalColumn: "rmc_patron_tiers_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "poll_votes",
                columns: table => new
                {
                    poll_votes_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    poll_id = table.Column<int>(type: "INTEGER", nullable: false),
                    poll_option_id = table.Column<int>(type: "INTEGER", nullable: false),
                    player_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    voted_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_votes", x => x.poll_votes_id);
                    table.ForeignKey(
                        name: "FK_poll_votes_player_player_user_id",
                        column: x => x.player_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_poll_votes_poll_options_poll_option_id",
                        column: x => x.poll_option_id,
                        principalTable: "poll_options",
                        principalColumn: "poll_options_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_poll_votes_polls_poll_id",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "polls_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_patron_lobby_messages",
                columns: table => new
                {
                    patron_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_patron_lobby_messages", x => x.patron_id);
                    table.ForeignKey(
                        name: "FK_rmc_patron_lobby_messages_rmc_patrons_patron_id",
                        column: x => x.patron_id,
                        principalTable: "rmc_patrons",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rmc_patron_round_end_nt_shoutouts",
                columns: table => new
                {
                    patron_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_patron_round_end_nt_shoutouts", x => x.patron_id);
                    table.ForeignKey(
                        name: "FK_rmc_patron_round_end_nt_shoutouts_rmc_patrons_patron_id",
                        column: x => x.patron_id,
                        principalTable: "rmc_patrons",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_poll_options_poll_id",
                table: "poll_options",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_seen_player_user_id",
                table: "poll_seen",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_seen_poll_id",
                table: "poll_seen",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_seen_poll_id_player_user_id",
                table: "poll_seen",
                columns: new[] { "poll_id", "player_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_poll_votes_player_user_id",
                table: "poll_votes",
                column: "player_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_votes_poll_id",
                table: "poll_votes",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_votes_poll_id_player_user_id_poll_option_id",
                table: "poll_votes",
                columns: new[] { "poll_id", "player_user_id", "poll_option_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_poll_votes_poll_option_id",
                table: "poll_votes",
                column: "poll_option_id");

            migrationBuilder.CreateIndex(
                name: "IX_polls_active",
                table: "polls",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "IX_polls_created_by_id",
                table: "polls",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_polls_end_time",
                table: "polls",
                column: "end_time");

            migrationBuilder.CreateIndex(
                name: "IX_polls_start_time",
                table: "polls",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_linked_accounts_discord_id",
                table: "rmc_linked_accounts",
                column: "discord_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rmc_linked_accounts_logs_at",
                table: "rmc_linked_accounts_logs",
                column: "at");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_linked_accounts_logs_discord_id",
                table: "rmc_linked_accounts_logs",
                column: "discord_id");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_linked_accounts_logs_player_id",
                table: "rmc_linked_accounts_logs",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_linking_codes_code",
                table: "rmc_linking_codes",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_patron_tiers_discord_role",
                table: "rmc_patron_tiers",
                column: "discord_role",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rmc_patron_tiers_lobby_message",
                table: "rmc_patron_tiers",
                column: "lobby_message");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_patron_tiers_round_end_shoutout",
                table: "rmc_patron_tiers",
                column: "round_end_shoutout");

            migrationBuilder.CreateIndex(
                name: "IX_rmc_patrons_tier_id",
                table: "rmc_patrons",
                column: "tier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "poll_seen");

            migrationBuilder.DropTable(
                name: "poll_votes");

            migrationBuilder.DropTable(
                name: "rmc_linked_accounts");

            migrationBuilder.DropTable(
                name: "rmc_linked_accounts_logs");

            migrationBuilder.DropTable(
                name: "rmc_linking_codes");

            migrationBuilder.DropTable(
                name: "rmc_patron_lobby_messages");

            migrationBuilder.DropTable(
                name: "rmc_patron_round_end_nt_shoutouts");

            migrationBuilder.DropTable(
                name: "poll_options");

            migrationBuilder.DropTable(
                name: "rmc_discord_accounts");

            migrationBuilder.DropTable(
                name: "rmc_patrons");

            migrationBuilder.DropTable(
                name: "polls");

            migrationBuilder.DropTable(
                name: "rmc_patron_tiers");

            migrationBuilder.DropColumn(
                name: "bark_voice",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "last_rolled_antag",
                table: "player");

            migrationBuilder.DropColumn(
                name: "server_currency",
                table: "player");

            migrationBuilder.AddColumn<byte[]>(
                name: "organ_markings",
                table: "profile",
                type: "jsonb",
                nullable: true);
        }
    }
}
