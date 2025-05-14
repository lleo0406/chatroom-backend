using BackEnd.Enums;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class ChatRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [EnumDataType(typeof(ChatRoomType))]
        public ChatRoomType Type { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        public string? Picture { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatRoomMembers> Members { get; set; } = new List<ChatRoomMembers>();

        public ICollection<Messages> Messages { get; set; } = new List<Messages>();

        public override string ToString()
        {
            return $"ChatRoom: {Id}, Name: {Name}, Type: {Type}, IsPrivate: {IsPrivate}, CreatedAt: {CreatedAt}";
        }

    }
}
