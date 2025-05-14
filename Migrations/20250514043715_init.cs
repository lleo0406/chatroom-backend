using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chatroom",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    isprivate = table.Column<bool>(type: "boolean", nullable: false),
                    picture = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chatroom", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    displayname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    displayid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    salt = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    picture = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    googleid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chatroommembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chatroomid = table.Column<int>(type: "integer", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    activation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chatroommembers", x => x.id);
                    table.ForeignKey(
                        name: "fk_chatroommembers_chatroom_chatroomid",
                        column: x => x.chatroomid,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chatroommembers_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    requesterid = table.Column<int>(type: "integer", nullable: false),
                    responderid = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requestedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_friends", x => x.id);
                    table.ForeignKey(
                        name: "fk_friends_users_requesterid",
                        column: x => x.requesterid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_friends_users_responderid",
                        column: x => x.responderid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    messagetype = table.Column<string>(type: "text", nullable: false),
                    fileurl = table.Column<string>(type: "text", nullable: true),
                    sendat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    chatroomid = table.Column<int>(type: "integer", nullable: false),
                    senderid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_chatroom_chatroomid",
                        column: x => x.chatroomid,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_senderid",
                        column: x => x.senderid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passwordresettoken",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    expirationtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isused = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passwordresettoken", x => x.id);
                    table.ForeignKey(
                        name: "fk_passwordresettoken_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chatroommembers_chatroomid",
                table: "chatroommembers",
                column: "chatroomid");

            migrationBuilder.CreateIndex(
                name: "ix_chatroommembers_userid",
                table: "chatroommembers",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_friends_requesterid_responderid",
                table: "friends",
                columns: new[] { "requesterid", "responderid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_friends_responderid",
                table: "friends",
                column: "responderid");

            migrationBuilder.CreateIndex(
                name: "ix_messages_chatroomid",
                table: "messages",
                column: "chatroomid");

            migrationBuilder.CreateIndex(
                name: "ix_messages_senderid",
                table: "messages",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "ix_passwordresettoken_userid",
                table: "passwordresettoken",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_users_displayid",
                table: "users",
                column: "displayid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatroommembers");

            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "passwordresettoken");

            migrationBuilder.DropTable(
                name: "chatroom");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
