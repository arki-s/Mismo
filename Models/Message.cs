using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mismo.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, 3)]
        public int Priority { get; set; }

        [Required]
        public string Comment { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

    }
}
