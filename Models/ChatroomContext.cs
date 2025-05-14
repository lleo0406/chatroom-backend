using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BackEnd.Models;

public class IChatroomContext : DbContext
{
    public IChatroomContext(DbContextOptions<IChatroomContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Friends> Friends { get; set; }
    public DbSet<ChatRoom> ChatRoom { get; set; }
    public DbSet<ChatRoomMembers> ChatRoomMembers { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<PasswordResetToken> PasswordResetToken { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DisplayId).IsUnique();
        });

        modelBuilder.Entity<Friends>(entity =>
        {
            entity.HasIndex(f => new { f.RequesterId, f.ResponderId }).IsUnique();

            entity.HasOne(f => f.Responder)
                  .WithMany(u => u.ReceivedFriendRequests)
                  .HasForeignKey(f => f.ResponderId)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<ChatRoomMembers>(entity =>
        {

            entity.HasOne(m => m.ChatRoom)
                  .WithMany(r => r.Members)
                  .HasForeignKey(m => m.ChatRoomId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.User)
                  .WithMany(u => u.JoinedChatRooms)
                  .HasForeignKey(m => m.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Messages>(entity =>
        {

            entity.HasOne(m => m.ChatRoom)
                  .WithMany(r => r.Messages)
                  .HasForeignKey(m => m.ChatRoomId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(m => m.MessageType)
                  .HasConversion<string>();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {

            entity.HasOne(p => p.User)
                  .WithMany()
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName(), null)).ToLower());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToLower());
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName().ToLower());
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToLower());
            }
        }

    }

}
