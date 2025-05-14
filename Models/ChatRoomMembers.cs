using BackEnd.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class ChatRoomMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ChatRoomId { get; set; }

        [ForeignKey("ChatRoomId")]
        public ChatRoom ChatRoom { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ActivationStatus Activation { get; set; } = ActivationStatus.NotActivated;
        public override string ToString()
        {
            return $"ChatRoomMembers: {Id}, ChatRoomId: {ChatRoomId}, UserId: {UserId}";
        }


    }
}
