using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mismo.Models
{
    public class OneOnOne
    {
        [Key]
        public int OneOnOneId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date {  get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public string Comment { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string UserUd {  get; set; }        

    }
}
