using BackEnd.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class Friends
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public User Requester { get; set; } = null!;

        [Required]
        public int ResponderId { get; set; }

        [ForeignKey("ResponderId")]
        public User Responder { get; set; } = null!;

        [Required]
        public FriendStatus Status { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }


    }
}
