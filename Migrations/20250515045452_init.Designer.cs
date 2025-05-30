﻿// <auto-generated />
using System;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackEnd.Migrations
{
    [DbContext(typeof(IChatroomContext))]
    [Migration("20250515045452_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BackEnd.Models.ChatRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdat");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean")
                        .HasColumnName("isprivate");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<string>("Picture")
                        .HasColumnType("text")
                        .HasColumnName("picture");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_chatroom");

                    b.ToTable("chatroom");
                });

            modelBuilder.Entity("BackEnd.Models.ChatRoomMembers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Activation")
                        .HasColumnType("integer")
                        .HasColumnName("activation");

                    b.Property<int>("ChatRoomId")
                        .HasColumnType("integer")
                        .HasColumnName("chatroomid");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.HasKey("Id")
                        .HasName("pk_chatroommembers");

                    b.HasIndex("ChatRoomId")
                        .HasDatabaseName("ix_chatroommembers_chatroomid");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_chatroommembers_userid");

                    b.ToTable("chatroommembers");
                });

            modelBuilder.Entity("BackEnd.Models.Friends", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("RequestedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("requestedat");

                    b.Property<int>("RequesterId")
                        .HasColumnType("integer")
                        .HasColumnName("requesterid");

                    b.Property<int>("ResponderId")
                        .HasColumnType("integer")
                        .HasColumnName("responderid");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_friends");

                    b.HasIndex("ResponderId")
                        .HasDatabaseName("ix_friends_responderid");

                    b.HasIndex("RequesterId", "ResponderId")
                        .IsUnique()
                        .HasDatabaseName("ix_friends_requesterid_responderid");

                    b.ToTable("friends");
                });

            modelBuilder.Entity("BackEnd.Models.Messages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatRoomId")
                        .HasColumnType("integer")
                        .HasColumnName("chatroomid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("FileUrl")
                        .HasColumnType("text")
                        .HasColumnName("fileurl");

                    b.Property<string>("MessageType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("messagetype");

                    b.Property<DateTime>("SendAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sendat");

                    b.Property<int>("SenderId")
                        .HasColumnType("integer")
                        .HasColumnName("senderid");

                    b.HasKey("Id")
                        .HasName("pk_messages");

                    b.HasIndex("ChatRoomId")
                        .HasDatabaseName("ix_messages_chatroomid");

                    b.HasIndex("SenderId")
                        .HasDatabaseName("ix_messages_senderid");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("BackEnd.Models.PasswordResetToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expirationtime");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean")
                        .HasColumnName("isused");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("token");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("userid");

                    b.HasKey("Id")
                        .HasName("pk_passwordresettoken");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_passwordresettoken_userid");

                    b.ToTable("passwordresettoken");
                });

            modelBuilder.Entity("BackEnd.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdat");

                    b.Property<string>("DisplayId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("displayid");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("displayname");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("GoogleId")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("googleid");

                    b.Property<string>("Password")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("password");

                    b.Property<string>("Picture")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("picture");

                    b.Property<string>("Salt")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("salt");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("DisplayId")
                        .IsUnique()
                        .HasDatabaseName("ix_users_displayid");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.ToTable("users");
                });

            modelBuilder.Entity("BackEnd.Models.ChatRoomMembers", b =>
                {
                    b.HasOne("BackEnd.Models.ChatRoom", "ChatRoom")
                        .WithMany("Members")
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chatroommembers_chatroom_chatroomid");

                    b.HasOne("BackEnd.Models.User", "User")
                        .WithMany("JoinedChatRooms")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chatroommembers_users_userid");

                    b.Navigation("ChatRoom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.Friends", b =>
                {
                    b.HasOne("BackEnd.Models.User", "Requester")
                        .WithMany()
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_friends_users_requesterid");

                    b.HasOne("BackEnd.Models.User", "Responder")
                        .WithMany("ReceivedFriendRequests")
                        .HasForeignKey("ResponderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_friends_users_responderid");

                    b.Navigation("Requester");

                    b.Navigation("Responder");
                });

            modelBuilder.Entity("BackEnd.Models.Messages", b =>
                {
                    b.HasOne("BackEnd.Models.ChatRoom", "ChatRoom")
                        .WithMany("Messages")
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_messages_chatroom_chatroomid");

                    b.HasOne("BackEnd.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_messages_users_senderid");

                    b.Navigation("ChatRoom");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("BackEnd.Models.PasswordResetToken", b =>
                {
                    b.HasOne("BackEnd.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_passwordresettoken_users_userid");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.ChatRoom", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("BackEnd.Models.User", b =>
                {
                    b.Navigation("JoinedChatRooms");

                    b.Navigation("ReceivedFriendRequests");

                    b.Navigation("SentMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
