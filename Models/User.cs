using System;
using System.Collections.Generic;

namespace BackEnd.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自動遞增
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string? Password { get; set; }

    [Required]
    [StringLength(255)]
    public string DisplayName { get; set; } = null!;

    [StringLength(255)]
    public string? DisplayId { get; set; }

    [StringLength(255)]
    public string? Salt { get; set; }

    [StringLength(255)]
    public string? Picture { get; set; }

    [StringLength(255)]
    public string? GoogleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Friends> ReceivedFriendRequests { get; set; } = new List<Friends>();

    public ICollection<ChatRoomMembers> JoinedChatRooms { get; set; } = new List<ChatRoomMembers>();

    public ICollection<Messages> SentMessages { get; set; } = new List<Messages>();

    public override string ToString()
    {
        return $"User: {Id}, Password: {Password}, Email: {Email}, DisplayName: {DisplayName}, CreatedAt: {CreatedAt}";
    }

}

